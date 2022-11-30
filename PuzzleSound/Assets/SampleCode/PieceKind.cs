using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceKind
{
    Red = 0,
    Blue,
    Green,
    Yellow,
    //Black,
    //Magenta,
    TapNote,
    FlicNote,
    LongNote,
    MusicNote,
} 

public enum FlicDir
{
    right = 0,
    left,
    up,
    dowm,
    nodir,
}

public enum CountLength
{
    zero = 0,
    one,
    two,
    three,
    four,
    five,
}