﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Box : MonoBehaviour
{
    public BoxData boxData;
    public GameObject inventoryIcon;
    public Color color;
    public Sprite icon;

    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public BoxCoordDictionary boxCoordDictionary;
    [HideInInspector]
    public Puzzle puzzle;

    [SerializeField]
    public string i_am = "m3Object";
    public string resourcePath;
    public int weight;
    public bool isTooHeavy;

    private bool isBeingChecked = false;
    protected bool[] wasNeighborChecked = new bool[6];
    public float distanceCheck = 1.5f;
    public Vector3 centerLocalTransform = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]
    public Vector3[] neighborCoordEvaluatorLocalTransforms = new Vector3[8]
    {
        new Vector3(0.1f,0.1f,0.1f),
        new Vector3(0.1f,0.1f,0.9f),
        new Vector3(0.1f,0.9f,0.1f),
        new Vector3(0.1f,0.9f,0.9f),
        new Vector3(0.9f,0.1f,0.1f),
        new Vector3(0.9f,0.1f,0.9f),
        new Vector3(0.9f,0.9f,0.1f),
        new Vector3(0.9f,0.9f,0.9f),
    };
    private static Vector3Int[] neighborLocalCoords = new Vector3Int[] {
        Vector3Int.up,
        Vector3Int.right,
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        Vector3Int.left,
        Vector3Int.down
    };

    [HideInInspector]
    public Vector3Int[] coords;
    [HideInInspector]
    public Vector3Int[] prev_coord;

    public GroupIdNames groupId;

    public enum GroupIdNames
    {
        Blue,
        Red,
        Green,
        Yellow
    }

    [SerializeField]
    private bool _frozen;
    public bool Frozen
    {
        get { return _frozen; }
        set
        {
            if (value)
            {
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            }
            else
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            _frozen = value;
        }
    }

    public MeshRenderer HighlightRenderer;

    void Awake()
    {
        grid = GetComponentInParent<Grid>();
        boxCoordDictionary = GetComponentInParent<BoxCoordDictionary>();
        if (!puzzle)
        {
            puzzle = GetComponentInParent<Puzzle>();
        }
    }

    void Start()
    {
        coords = new Vector3Int[neighborCoordEvaluatorLocalTransforms.Length];
        prev_coord = new Vector3Int[neighborCoordEvaluatorLocalTransforms.Length];
        //Initialize coord
        for (int i = 0; i < neighborCoordEvaluatorLocalTransforms.Length; ++i)
        {
            coords[i] = grid.WorldToCell(transform.position + transform.rotation * neighborCoordEvaluatorLocalTransforms[i]);
            boxCoordDictionary.Add(coords[i], gameObject);
        }
        if (puzzle)
        {
            puzzle.originalBoxList.Add(this);
        }
        coords.CopyTo(prev_coord, 0);
        Frozen = _frozen;
    }
    public virtual void FixedUpdate()
    {
        UpdateCoords();
    }

    private void OnDestroy()
    {
        RemoveMyself();
        if (puzzle)
        {
            puzzle.originalBoxList.Remove(this);
        }
    }
    void UpdateCoords()
    {
        AddMyself();
        for (int j = 0; j < prev_coord.Length; ++j)
        {
            bool found = false;
            for (int k = 0; k < coords.Length; ++k)
            {
                if (prev_coord[j] == coords[k])
                {
                    found = true;
                }
            }
            if (!found)
            {
                //This coord
                //Debug.Log("Cell removed");
                boxCoordDictionary.Remove(prev_coord[j], transform.gameObject);
            }
        }
        coords.CopyTo(prev_coord, 0);

    }

    //Returns a list of matching neighbors
    public List<Box> GetMatchingNeighbors()
    {
        List<Box> matching_neigbors = new List<Box>();
        isBeingChecked = true;
        for (int i = 0; i < coords.Length; ++i)
        {
            for (int j = 0; j < neighborLocalCoords.Length; ++j)
            {
                ////Iterate through all neighbors
                //if (wasNeighborChecked[j] == true)
                //{
                //    //Skip this neighbor (because this neighbor made the recursive call)
                //    continue;
                //}
                //Get neighbor from match3_grid
                GameObject[] neighbor_game_objects = boxCoordDictionary.Get(coords[i] + neighborLocalCoords[j]);

                if (neighbor_game_objects != null)
                {
                    for (int k = 0; k < neighbor_game_objects.Length; ++k)
                    {
                        //Neighbors exist
                        if (neighbor_game_objects[k] != null)
                        {
                            Box neighbor = neighbor_game_objects[k].GetComponent<Box>();
                            if (!neighbor.isBeingChecked && neighbor.groupId == groupId)
                            {
                                //Neighbor is of the same group
                                //Check if neighbor is close enough
                                if (GetDistanceToNeighbor(neighbor) < distanceCheck)
                                {
                                    //neighbor.wasNeighborChecked[neighborLocalCoords.Length - (j + 1)] = true;
                                    //wasNeighborChecked[j] = true;
                                    neighbor.GetMatchingNeighborsHelper(matching_neigbors);
                                }
                            }
                        }
                        else
                        {
                            boxCoordDictionary.Remove(coords[i] + neighborLocalCoords[j], neighbor_game_objects[k]);
                        }
                        
                    }
                }

            }
        }
        matching_neigbors.Add(this);
        foreach (Box obj in matching_neigbors)
        {
            obj.ResetChecked();
        }

        return matching_neigbors;
    }

    //Helper function for finding matching neighbors
    private void GetMatchingNeighborsHelper(List<Box> matching_neigbors)
    {
        isBeingChecked = true;
        for (int i = 0; i < coords.Length; ++i)
        {
            for (int j = 0; j < neighborLocalCoords.Length; ++j)
            {
                ////Iterate through all neighbors
                //if (wasNeighborChecked[j] == true)
                //{
                //    //Skip this neighbor (because this neighbor made the recursive call)
                //    continue;
                //}
                //Get neighbor from match3_grid
                GameObject[] neighbor_game_objects = boxCoordDictionary.Get(coords[i] + neighborLocalCoords[j]);

                if (neighbor_game_objects != null)
                {
                    for (int k = 0; k < neighbor_game_objects.Length; ++k)
                    {
                        if (neighbor_game_objects[k] != null)
                        {

                            //Neighbor exists
                            Box neighbor = neighbor_game_objects[k].GetComponent<Box>();
                            if (!neighbor.isBeingChecked && neighbor.groupId == groupId)
                            {
                                //Neighbor is of the same group
                                //Check if neighbor is close enough
                                if (GetDistanceToNeighbor(neighbor) < distanceCheck)
                                {
                                    //neighbor.wasNeighborChecked[neighborLocalCoords.Length - (j + 1)] = true;
                                    //wasNeighborChecked[j] = true;
                                    neighbor.GetMatchingNeighborsHelper(matching_neigbors);
                                }
                            }

                        }
                        else
                        {
                            boxCoordDictionary.Remove(coords[i] + neighborLocalCoords[j], neighbor_game_objects[k]);

                        }
                    }
                }
            }
        }
        matching_neigbors.Add(this);
    }

    private float GetDistanceToNeighbor(Box neighbor)
    {
        return Vector3.Distance(neighbor.transform.position + neighbor.transform.rotation * neighbor.centerLocalTransform, transform.position + transform.rotation * centerLocalTransform);
    }

    public int GetStackWeight()
    {
        //Get coord with highest y value
        Vector3Int hiCoord = GetHighestCoordAlignedWithStack();
        //Get the neighbors in the cell above me
        GameObject[] neighborGameObjects = boxCoordDictionary.Get(hiCoord + Vector3Int.up);
        Box neighbor = neighborGameObjects?[0].GetComponent<Box>();
        if (neighbor != null)
        {
            return weight + neighbor.GetStackWeight();
        }
        else
        {
            return weight;
        }

    }

    public Box GetBoxOnTopOfMe()
    {
        //Get coord with highest y value
        Vector3Int hiCoord = GetHighestCoordAlignedWithStack();
        //Get the neighbors in cell above me
        GameObject[] neighbors = boxCoordDictionary.Get(hiCoord + Vector3Int.up);
        if (neighbors != null)
        {
            foreach (GameObject neighbor in neighbors)
            {
                if (gameObject != neighbor.gameObject)
                {
                    return neighbor.GetComponent<Box>();
                }
            }
            return null;
        }
        else
        {
            return null;
        }
        
        
    }

    public Box GetBoxOnTopOfMyStack()
    {
        Box neighbor = GetBoxOnTopOfMe();
        if (neighbor == null)
        {
            return this;
        }
        else
        {
            return neighbor.GetBoxOnTopOfMyStack();
        }
        
    }

    public Vector3Int GetHighestCoordAlignedWithStack()
    {
        float hiY = coords[0].y;
        float avgX = 0;
        float avgZ = 0;
        foreach (Vector3Int coord in coords)
        {
            avgX += coord.x;
            avgZ += coord.z;
            if (coord.y > hiY)
                hiY = coord.y;
        }
        avgX = avgX / coords.Length;
        avgZ = avgZ / coords.Length;
        return new Vector3Int(Mathf.RoundToInt(avgX), (int)hiY, Mathf.RoundToInt(avgZ));
    }
    public void ResetChecked()
    {
        isBeingChecked = false;
        //for (int i = 0; i < wasNeighborChecked.Length; ++i)
        //{
        //    wasNeighborChecked[i] = false;
        //}
    }
    public void RemoveMyself()
    {
        for (int i = 0; i < coords.Length; ++i)
        {
            boxCoordDictionary.Remove(coords[i], transform.gameObject);
            boxCoordDictionary.Remove(prev_coord[i], transform.gameObject);
        }
    }

    public void AddMyself(bool forceAdd = false)
    {
        for (int i = 0; i < neighborCoordEvaluatorLocalTransforms.Length; ++i)
        {
            coords[i] = grid.WorldToCell(transform.position + (transform.rotation * neighborCoordEvaluatorLocalTransforms[i]));
            if (forceAdd == true || coords[i] != prev_coord[i])
            {
                //Debug.Log("Cell added");
                boxCoordDictionary.Add(coords[i], gameObject);
            }
        }
    }

    [ContextMenu("Snap To Whole Coordinate")]
    public void SnapToWholeCoordinate()
    {
        Grid _grid = GetComponentInParent<Grid>();
        transform.position = _grid.CellToWorld(_grid.WorldToCell(transform.position));
    }

    public void Highlight(bool highlight = true)
    {
        HighlightRenderer.enabled = highlight;
    }
}
