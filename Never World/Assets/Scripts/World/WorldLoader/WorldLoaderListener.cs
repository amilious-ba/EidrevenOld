using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WorldLoaderListener{
   void updateStatus(int value, int maxValue, string status);
}
