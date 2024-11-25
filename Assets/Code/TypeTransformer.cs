using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTransformer : MonoBehaviour
{
    public static byte[] SerializeColor(object p_obj)
    {
        Color m_color = (Color)p_obj;
        byte[] bytes = new byte[16];
        int index = 0;
        Protocol.Serialize(m_color.r, bytes, ref index);
        Protocol.Serialize(m_color.g, bytes, ref index);
        Protocol.Serialize(m_color.b, bytes, ref index);
        Protocol.Serialize(m_color.a, bytes, ref index);

        return bytes;
    }

    public static object DeserializeColor(byte[] bytes)
    {
        Color m_color = new Color();
        int index = 0;
        Protocol.Deserialize(out m_color.r, bytes, ref index);
        Protocol.Deserialize(out m_color.g, bytes, ref index);
        Protocol.Deserialize(out m_color.b, bytes, ref index);
        Protocol.Deserialize(out m_color.a, bytes, ref index);

        return m_color;
    }
}
