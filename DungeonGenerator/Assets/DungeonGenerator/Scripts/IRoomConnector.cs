using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomConnector
{
    void SetUpConnection(GameObject exit);
    void ExitRoom(GameObject exitingObject);
}
