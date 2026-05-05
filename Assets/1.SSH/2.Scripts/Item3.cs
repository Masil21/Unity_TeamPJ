using UnityEngine;

public class Item3 : MonoBehaviour
{
    public enum ItemType
    {
        None = -1,
        Coin,
        Boom,
        Power
    }

    public ItemType itemType = ItemType.Coin;
}
