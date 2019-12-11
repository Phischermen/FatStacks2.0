﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Player character;
    public ArsenalSystem gunController;
    public enum PickupState
    {
        noObjectTargeted,
        pickupObjectTargeted,
        interactObjectTargeted,
    }
    [HideInInspector]
    public PickupState currentPickupState = PickupState.noObjectTargeted;
    public float distance = 3;
    public float distanceMin = 2;
    public float distanceMax = 5;

    public bool snap;
    public bool showPreview;
    Vector3Int[] dropCoords = new Vector3Int[2];
    Vector3[] dropLocations = new Vector3[2];
    bool[] canDropAtCoords = new bool[] { true, true };
    bool[] showDrop = new bool[] { true, true };

    [HideInInspector]
    public GameObject targetedObject = null;
    private Interaction targetedItemInteraction;
    private Box targetedItemBox;
    [HideInInspector]
    public Box carriedItem = null;
    public Stack<Box> carriedObjects = new Stack<Box>();
    public int carryObjectLimit;
    public BoxInventoryDisplay boxInventoryDisplay;
    private bool wasPickupPressed;
    private bool wasDropOnStackPressed;
    private Rigidbody itemRigidbody;
    public PickupPreview[] placementPreviews;
    private Mesh carriedItemMesh;
    private Material carriedItemMaterial;
    [HideInInspector]
    public Grid placementGrid;
    //public MatchThreeGridDataStructure structure;
    public Fade prompt;
    public Fade exception;
    public Text snapText;
    public Text previewText;
    private int layerMaskPickup;
    private int layerMaskObstructed;
    private int layerMaskHide;

    void Awake()
    {
        layerMaskPickup = LayerMask.GetMask("InteractSolid", "InteractSoft", "Default");
        layerMaskObstructed = LayerMask.GetMask("Player");
        layerMaskHide = LayerMask.GetMask("InteractSolid", "Default");
        snapText.text = "<b>[X]Snap: </b>" + ((snap) ? "ON" : "OFF");
        previewText.text = "<b>[V]Preview: </b>" + ((showPreview) ? "ON" : "OFF");
        //character = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Check for collision with liftable objects
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
        bool objectFound = Physics.Raycast(ray, out hitInfo, distanceMax, layerMaskPickup);
        if (Input.GetButtonDown("Snap"))
        {
            snap = !snap;
            snapText.text = "<b>[X]Snap: </b>" + ((snap) ? "ON" : "OFF");
        }
        if (Input.GetButtonDown("ShowPreview"))
        {
            showPreview = !showPreview;
            previewText.text = "<b>[V]Preview: </b>" + ((showPreview) ? "ON" : "OFF");
        }

        switch (currentPickupState)
        {
            case PickupState.noObjectTargeted:
                if (objectFound)
                {
                    targetedObject = hitInfo.transform.gameObject;
                    bool busy = true;
                    PickupState nextState = PickupState.noObjectTargeted;
                    switch (hitInfo.transform.tag)
                    {
                        case "Interactable":
                            nextState = PickupState.interactObjectTargeted;
                            targetedItemInteraction = targetedObject.GetComponent<Interaction>();
                            busy = targetedItemInteraction.IsBusy();
                            if(!busy)
                                RefreshText();
                            break;
                        case "Liftable":
                            nextState = PickupState.pickupObjectTargeted;
                            targetedItemBox = targetedObject.GetComponent<Box>();
                            busy = false;
                            prompt.fadeInText("<b>[E]</b>LIFT");
                            break;
                        default:
                            break;
                    }
                    //Check the availability of the targeted object
                    if (busy == false)
                    {
                        currentPickupState = nextState;
                        //Debug.Log("Target found.");
                        //Debug.Log("Targeted item: " + targetedObject.name);
                    }
                }


                break;
            case PickupState.interactObjectTargeted:
                if (InteractTargetLost(objectFound, hitInfo))
                {
                    //Object not targeted anymore
                    LoseTarget();
                }
                else
                {
                    if (Input.GetButtonDown("Interact/Pickup"))
                    {
                        //Interact with object
                        targetedItemInteraction.Interact(this);
                    }
                }
                break;
            case PickupState.pickupObjectTargeted:
                if (PickupTargetLost(objectFound, hitInfo))
                {
                    LoseTarget();
                }
                else
                {
                    bool pickup = false;
                    Box box = targetedItemBox;
                    if (Input.GetButtonDown("Interact/Pickup"))
                    {
                        //Pickup object
                        pickup = true;
                    }
                    if (Input.GetButtonDown("PickupOnStack"))
                    {
                        //Pickup object on top of a stack
                        pickup = true;
                        box = targetedItemBox.GetBoxOnTopOfMyStack();
                    }
                    if (pickup)
                    {
                        if (box.isTooHeavy)
                        {
                            exception.FlashText("TOO HEAVY", 2f);
                        }
                        else if (carriedObjects.Count == carryObjectLimit)
                        {
                            exception.FlashText("INVENTORY FULL", 2f);
                        }
                        //else if (box.GetBoxOnTopOfMe() != null)
                        //{

                        //    exception.FlashText("BOX BLOCKED", 2f);
                        //}
                        else
                        {
                            PickupObject(box);
                        }
                    }
                }
                break;
        }
        if(carriedObjects.Count > 0 && currentPickupState != PickupState.interactObjectTargeted)
        { 
            //Check distance mod
            float scrollDelta = Input.GetAxis("DistanceModification");
            if (scrollDelta != 0)
            {
                distance = Mathf.Clamp(distance + scrollDelta, distanceMin, distanceMax);
            }

            //Set initial drop location
            dropCoords = new Vector3Int[] { Vector3Int.zero, Vector3Int.zero };
            dropLocations = new Vector3[] { Vector3.zero, Vector3.zero };
            canDropAtCoords = new bool[] { true, true };
            showDrop = new bool[] { true, true };

            
            if (objectFound && hitInfo.distance <= distance)
            {
                dropLocations[0] = hitInfo.point + (hitInfo.normal * 0.53f) - (snap ? Vector3.zero : carriedItem.centerLocalTransform);
                dropCoords[0] = placementGrid.WorldToCell(dropLocations[0]);
                Box box = hitInfo.transform.gameObject.GetComponent<Box>();
                if (box != null)
                {
                    dropCoords[1] = box.GetBoxOnTopOfMyStack().GetHighestCoordAlignedWithStack() + Vector3Int.up;
                    canDropAtCoords[1] = Vector3.Distance(transform.position, placementGrid.CellToWorld(dropCoords[1])) < distanceMax;
                }
                else
                {
                    canDropAtCoords[1] = false;
                    showDrop[1] = false;
                }
            }
            else
            {
                dropLocations[0] = transform.position + (transform.rotation * (Vector3.forward * distance) - carriedItem.centerLocalTransform);
                dropCoords[0] = placementGrid.WorldToCell(dropLocations[0] + carriedItem.centerLocalTransform);
                canDropAtCoords[1] = false;
                showDrop[1] = false;
            }
            //Set pickup preview materials
            if(!canDropAtCoords[1] || dropCoords[0] == dropCoords[1])
            {
                placementPreviews[0].SetPreview(carriedItem, PickupPreview.Prompt.r);
                showDrop[1] = false;
            }
            else
            {
                placementPreviews[0].SetPreview(carriedItem, PickupPreview.Prompt.r);
                placementPreviews[1].SetPreview(carriedItem, PickupPreview.Prompt.g);
            }
            
            //Convert back to world space
            for (int i = 0; i < dropCoords.Length; ++i)
            {
                if(i > 0 || snap)
                {
                    dropLocations[i] = placementGrid.CellToWorld(dropCoords[i]);
                }
                bool obstructionNotDetected = !Physics.CheckBox(dropLocations[i] + new Vector3(0.5f, 0.55f, 0.5f), new Vector3(0.51f, 0.475f, 0.51f), Quaternion.identity, layerMaskObstructed);
                bool clippingNotDetected = !Physics.CheckBox(dropLocations[i] + new Vector3(0.5f, 0.55f, 0.5f), new Vector3(0.51f, 0.475f, 0.51f), Quaternion.identity, layerMaskHide);
                canDropAtCoords[i] = canDropAtCoords[i] && obstructionNotDetected && clippingNotDetected;
                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                if (showDrop[i] && showPreview)
                {
                    placementPreviews[i].gameObject.SetActive(true);
                    if (canDropAtCoords[i])
                    {
                        prompt.fadeInText("<b>[R]</b>PLACE");
                        placementPreviews[i].SetValid(true);
                        //properties.SetColor("_Color", new Color(0.7f, 0.89f, 1f, 0.75f));
                    }
                    else
                    {
                        prompt.fadeOutText();
                        placementPreviews[i].SetValid(false);
                        //properties.SetColor("_Color", new Color(1f, 0.89f, 0.7f, 0.75f));
                    }
                    placementPreviews[i].gameObject.transform.position = dropLocations[i];
                    placementPreviews[i].gameObject.transform.rotation = Quaternion.identity;
                    //Graphics.DrawMesh(carriedItemMesh, dropLocations[i], Quaternion.identity, carriedItemMaterial, 0, GetComponent<Camera>(), 0, properties, false);
                }
                else
                {
                    placementPreviews[i].gameObject.SetActive(false);
                }
            }
            if ((Input.GetButtonDown("Drop") || (Input.GetButtonDown("DropOnStack") && !canDropAtCoords[1])) && canDropAtCoords[0])
            {
                DropObject(dropLocations[0], Quaternion.identity);
            }
            if (Input.GetButtonDown("DropOnStack") && canDropAtCoords[1])
            {
                DropObject(dropLocations[1], Quaternion.identity);
            }
        }
        else
        {
            placementPreviews[0].gameObject.SetActive(false);
            placementPreviews[1].gameObject.SetActive(false);
        }
        
        


        //if Input.GetButtonDown("Pickup"){

        //}
    }
    private void FixedUpdate()
    {
        //switch (currentPickupState)
        //{


        //    case PickupState.carryingObject:
        //        if (wasPickupPressed)
        //        {
        //            DropObject(dropLocations[0]);
        //            wasPickupPressed = false;
        //        }
        //        if (wasDropOnStackPressed)
        //        {
        //            DropObject(dropLocations[1]);
        //            wasDropOnStackPressed = false;
        //        }
        //        break;
        //    case PickupState.constrainingObject:
        //        //Transfering object to another grid.
        //        Box m3_object = carriedItem.GetComponent<Box>();
        //        if (m3_object._Grid != placementGrid)
        //        {
        //            m3_object.RemoveMyself();
        //            m3_object._Grid = placementGrid;
        //            //m3_object.match3_grid = structure;
        //            m3_object.AddMyself(true);
        //            m3_object.transform.SetParent(placementGrid.transform);
        //        }
        //        itemRigidbody = carriedItem.GetComponent<Rigidbody>();
        //        m3_object.Frozen = false;
        //        currentPickupState = PickupState.noObjectTargeted;
        //        carriedItem = null;
        //        break;
        //}

    }

    public void PickupObject(Box lift)
    {
        carriedItem = lift;
        carriedObjects.Push(lift);
        boxInventoryDisplay.AddBox(lift.inventoryIcon);
        SetCarriedItemMeshMaterialAndRigidbody(lift);
        itemRigidbody.velocity = Vector3.zero;
        // Debug.Log("Object carried: " + carried_item.name);
        lift.RemoveMyself();
        lift.gameObject.SetActive(false);
        //distance = distanceMax;
        //distance = Vector3.Distance(transform.position,obj.transform.position);

    }

    public void DropObject(Vector3 location, Quaternion rotation)
    {
        placementPreviews[0].gameObject.SetActive(false);
        placementPreviews[1].gameObject.SetActive(false);
        Box droppedItem = carriedObjects.Pop();
        boxInventoryDisplay.RemoveBox();
        if (carriedObjects.Count > 0)
            carriedItem = carriedObjects.Peek();
            //SetCarriedItemMeshMaterialAndRigidbody(carriedObjects.Peek());
        //Transfering object to another grid.
        if (droppedItem.grid != placementGrid)
        {
            droppedItem.grid = placementGrid;
            droppedItem.transform.SetParent(placementGrid.transform);
        }
        droppedItem.AddMyself(true);
        droppedItem.gameObject.SetActive(true);
        //Remove constraints of rigidbody
        droppedItem.transform.position = location;
        droppedItem.transform.rotation = rotation;
        
        droppedItem.Frozen = false;
    }

    private void SetCarriedItemMeshMaterialAndRigidbody(Box carriedItem)
    {
        carriedItemMesh = carriedItem.GetComponent<MeshFilter>().mesh;
        carriedItemMaterial = carriedItem.GetComponent<MeshRenderer>().sharedMaterials[0];
        itemRigidbody = carriedItem.GetComponent<Rigidbody>();
    }

    public void RefreshText()
    {
        prompt.fadeInText("<b>[E]</b>" + targetedItemInteraction.GetPrompt());
    }

    private bool InteractTargetLost(bool objectFound,RaycastHit hit)
    {
            return !objectFound || (hit.transform.gameObject != targetedObject) || targetedItemInteraction.IsBusy() == true;
    }

    private bool PickupTargetLost(bool objectFound,RaycastHit hit)
    {
            return !objectFound || hit.transform.gameObject != targetedObject;
    }

    private void LoseTarget()
    {
        //Object not targeted anymore
        prompt.fadeOutText();
        currentPickupState = PickupState.noObjectTargeted;
    }
}
