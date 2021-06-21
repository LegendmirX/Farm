using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using UnityEngine;

public class Inventory : IXmlSerializable
{
    public string Name { get; protected set; }
    public int Capacity { get; protected set; }
    
    public Dictionary<int, InventoryItem> items;

    public Inventory()
    {
        items = new Dictionary<int, InventoryItem>();
    }

    public Inventory(int capacity, string name = "NoName")
    {
        items = new Dictionary<int, InventoryItem>();
        this.Capacity = capacity;
        Name = name;

        for (int i = 0; i < capacity; i++)
        {
            items.Add(i, new InventoryItem()); 
        }
    }

    public void ChangeCapacity(int amount)
    {
        Capacity += amount;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                                                                        ///
    ///                                     SAVING & LOADING                                                   ///
    ///                                                                                                        ///
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer) //Save
    {        
        writer.WriteAttributeString("Capacity", Capacity.ToString());

        if(items != null)
        {
            List<int> keys = new List<int>();
            foreach(int key in items.Keys)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("SlotID", key.ToString());
                items[key].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }

    public void ReadXml(XmlReader reader) //Load
    {
        do
        {
            reader.MoveToAttribute("SlotID");
            int slotID = reader.ReadContentAsInt();
            reader.MoveToAttribute("SubType");
            string subType = reader.ReadContentAsString();
            reader.MoveToAttribute("Quantity");
            int amount = reader.ReadContentAsInt();

            //InventoryItem item = World.current.CreateInventoryItem(subType, amount);

        }
        while (reader.ReadToNextSibling("Item"));


    }
}
