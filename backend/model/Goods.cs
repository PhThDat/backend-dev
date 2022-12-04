namespace BackEndCSharp.Model;

class Goods
{
    public int ID;
    public string Name;
    public string Description;
    public string ImagePath;
}

class GoodsInfo
{
    public Goods Goods;
    public int Amount;
}