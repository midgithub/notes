using XLua;
﻿using UnityEngine;
using System.Collections;

public interface ParentPanelInterface {

    void OnSubPanelCall<T>(BaseSubPanel<T> subPanel,params object[] parame);

    //void SetSubPanel<T>(BaseSubPanel<T> subPanel,params object[] parame);
}



