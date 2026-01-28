using My_DS_Modules;

LinkList<int> linkList = new();
for (int i = 0; i < 10; i++)
  linkList.AddLast(i);

Console.Write("LinkList: ");
foreach (var item in linkList.Forward())
  Console.Write(item + " ");
Console.Write($" Count: {linkList.Count}");
Console.WriteLine();

linkList.AddFirst(10);
linkList.AddLast(11);

Console.Write("LinkList: ");
foreach (var item in linkList.Backward())
  Console.Write(item + " ");
Console.Write($" Count: {linkList.Count}");
Console.WriteLine();

Console.Write(linkList.Contains(7) + " ");
Console.Write(linkList.Contains(12));
Console.WriteLine();

// Could possible be null, change accept null ref: when insert is null AddLast || AddFirst both works
linkList.InsertAfter(linkList.Find(3), 8);
linkList.InsertBefore(linkList.Find(9), 14);
Console.Write("LinkList:");
foreach (var item in linkList.Forward())
  Console.Write($" {item}");
Console.Write($" Count: {linkList.Count}");
Console.WriteLine();

linkList.Remove(4);
Console.Write($"Removed Contains?: {linkList.Contains(4)}, Count: {linkList.Count}");
Console.WriteLine();
Console.Write("LinkList:");
foreach (var item in linkList.Forward())
  Console.Write($" {item}");
Console.Write($" Count: {linkList.Count}");
Console.WriteLine();

linkList.Clear();
Console.Write("LinkList:");
foreach (var item in linkList.Forward())
  Console.Write($" {item}");
Console.Write($" Count: {linkList.Count}");