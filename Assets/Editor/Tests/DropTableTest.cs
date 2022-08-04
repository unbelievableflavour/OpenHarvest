using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class DropTableTest
    {
        [Test]
        public void ItGetsARandomItemFromTheTable()
        {
            bool itemExistsInTable(List<DroppedItem> dropTableItems, GameObject itemToCheck)
            {
                foreach (DroppedItem drop in dropTableItems)
                {
                    if (drop.item == itemToCheck)
                    {
                        return true;
                    }
                }
                return false;
            }

            var dropTable = new DropTable();
            dropTable.items.Add(new DroppedItem()
            {
                dropRate  = 10,
                item = new GameObject()
            });
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 10,
                item = new GameObject()
            });

            Assert.AreEqual(true, itemExistsInTable(dropTable.items, dropTable.GetItemByDropRate()));
        }

        [Test]
        public void ItReturnsTheTotalDropRateCount()
        {
            var dropTable = new DropTable();
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 10,
                item = new GameObject()
            });
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 10,
                item = new GameObject()
            });
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 15,
                item = new GameObject()
            });

            Assert.AreEqual(35, dropTable.getTotalDropRateCount());
        }

        [Test]
        public void ItReturnsTheCorrectItemBasedOnARandomNumber()
        {
            var dropTable = new DropTable();
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 5,
                item = new GameObject()
            });
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 15,
                item = new GameObject()
            });
            dropTable.items.Add(new DroppedItem()
            {
                dropRate = 10,
                item = new GameObject()
            });

            Assert.AreEqual(dropTable.items[0].item, dropTable.GetItem(4));
            Assert.AreEqual(dropTable.items[0].item, dropTable.GetItem(5));
            Assert.AreEqual(dropTable.items[1].item, dropTable.GetItem(6));
            Assert.AreEqual(dropTable.items[1].item, dropTable.GetItem(14));
            Assert.AreEqual(dropTable.items[1].item, dropTable.GetItem(15));
            Assert.AreEqual(dropTable.items[1].item, dropTable.GetItem(16));
            Assert.AreEqual(dropTable.items[1].item, dropTable.GetItem(20));
            Assert.AreEqual(dropTable.items[2].item, dropTable.GetItem(21));
            Assert.AreEqual(dropTable.items[2].item, dropTable.GetItem(30));
            Assert.AreEqual(null, dropTable.GetItem(31));
        }
    }
}
