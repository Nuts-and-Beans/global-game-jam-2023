using System;
using Unity.Mathematics;

[Serializable]
public class Array2D<T>
{
    // --- DATA --- //
    [Serializable]
    public class InternalArray
    {
        public T[] yData;
    }
    
    public InternalArray[] xData;
    
    // --- CTORS --- //
    public Array2D(int2 length)
    {
        xData = new InternalArray[length.x];

        for (int i = 0; i < length.x; i++)
        {
            xData[i] = new InternalArray
            {
                yData = new T[length.y]
            };
        }
    }
    
    public Array2D(int lengthX, int lengthY)
    {
        xData = new InternalArray[lengthX];

        for (int i = 0; i < lengthX; i++)
        {
            xData[i].yData = new T[lengthY];
        }
    }

    // --- INDEXERS --- //
    public T this[int x, int y]
    {
        get => xData[x].yData[y];
        set => xData[x].yData[y] = value;
    }
    
    public T this[int2 i] 
    {
        get => xData[i.x].yData[i.y];
        set => xData[i.x].yData[i.y] = value;
    }
    
    // --- GETTERS --- //
    public int2 Length => new int2 { x = xData == null ? 0 : xData.Length, y = xData == null || xData.Length <= 0 ? 0 : xData[0].yData.Length };
}