using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
public class CarController : MonoBehaviour {
    private World.WorldCoord destination;
    private enum DEST_POS {
        LEFT_FRONT = 0,
        RIGHT_FRONT,
        LEFT_BACK,
        RIGHT_BACK
    };
    
    private void SetDest(World.WorldCoord i_destination) {
        destination = i_destination;
    }

    private void UpdateCar(NPCCar car) {
        if (car.transform.position != (Vector3)car.GetDest()) {
            return;
        }

        World.WorldCoord shift;
        shift = destination - car.GetLocation();
        
        World.WorldCoord[] possibleDirections = {
            car.GetDirection(),
            new World.WorldCoord(shift.x==0 ? 0:shift.x/Mathf.Abs(shift.x), 0),
            new World.WorldCoord(0, shift.y==0 ? 0:shift.y/Mathf.Abs(shift.y))
        };
        World.WorldCoord chosenDirection = car.GetDirection();
        
        if (car.GetDirection().x * shift.x < 0) {
            chosenDirection = possibleDirections[2];
        } else if (car.GetDirection().y * shift.y < 0) {
            chosenDirection = possibleDirections[1];
        } else {
            if (car.GetDirection().y == 0 && Mathf.Abs(shift.x) < 1.5f) {
                chosenDirection = possibleDirections[2];
            } else if (car.GetDirection().x == 0 && Mathf.Abs(shift.x) < 1.5f) {
                chosenDirection = possibleDirections[1];
            }
            
        }

        if (World.IsDirection(chosenDirection)==false || World.Instance.CanMoveInto(car.GetLocation() + chosenDirection, chosenDirection) == false) {
            chosenDirection = car.GetDirection();        
        }

        if(World.Instance.IsParkingSpot(car.GetLocation()+chosenDirection))
        {
            car.Park(chosenDirection);
        }
        else
        {
            car.MoveIfPossible(chosenDirection);
        }
    }
    
    public void Start() {
        
    }
    public void Update() {
        // Find an empty spot.
        World.WorldCoord parkingSpot = new World.WorldCoord(0, 0);
        List<World.WorldCoord> parkingSpots = new List<World.WorldCoord>();
        for (int i = 0; i < World.WORLD_WIDTH; ++ i) {
            for (int j = 0; j < World.WORLD_HEIGHT; ++j) {
                parkingSpot = new World.WorldCoord(i, j);
                if(World.Instance.IsParkingSpot(parkingSpot)
                        && (World.Instance.ParkingSpotOpen(parkingSpot, World.POSSIBLE_DIRECTIONS[0])
                            || World.Instance.ParkingSpotOpen(parkingSpot, World.POSSIBLE_DIRECTIONS[1]) 
                            || World.Instance.ParkingSpotOpen(parkingSpot, World.POSSIBLE_DIRECTIONS[2]) 
                            || World.Instance.ParkingSpotOpen(parkingSpot, World.POSSIBLE_DIRECTIONS[3]))) {
                    parkingSpots.Add(parkingSpot);
                    break;
                }
            }
        }

        NPCCar[] npcCars = FindObjectsOfType(typeof(NPCCar)) as NPCCar[];
        foreach(NPCCar npcCar in npcCars) {
            // Go to the nearest empty spot.
            int idxNearest = 0;
            int minDistance = int.MaxValue;
            for (int i = 0; i < parkingSpots.Count; ++ i) {
                int distance = Mathf.Abs(parkingSpots[i].x - npcCar.GetLocation().x) + Mathf.Abs(parkingSpots[i].y - npcCar.GetLocation().y);
                if (distance < minDistance) {
                    idxNearest = i;
                }
            }
            if (parkingSpots.Count > 0) {
                SetDest(parkingSpots[idxNearest]);
                UpdateCar(npcCar);
            } else {
                // Fixed the NPC cars don't move when there is no spot.
                if (npcCar.transform.position == (Vector3)npcCar.GetDest()) {
                    npcCar.MoveRandomDirection();
                }
            }
        }
    }
}
