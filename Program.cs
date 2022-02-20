using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab1_CSharp
{
    struct DataItem {
        public Vector2 Point{get; set;}
        public Complex Value{get; set;}

        public DataItem(Vector2 point, Complex value)
        {
            Point = point;
            Value = value;
        }

        public string ToLongString(string format) => $"Point: {Point.ToString(format)} | Value: {Value.ToString(format)}";

        public override string ToString() => $"Point: {Point} | Value: {Value}";
    }

    public delegate Complex Fv2Complex(Vector2 v2);

    abstract class V2Data 
    {
        public string ID{get;}
        public DateTime Date{get;}

        public V2Data(string id, DateTime date)
        {
            ID = id;
            Date = date;
        }

        public abstract int Count{get;}
        public abstract float MinDistance{get;}

        public abstract string ToLongString(string fromat);
        public override string ToString() => $"ID: {ID} | Date: {Date}";
    }

    class V2DataList : V2Data
    {
        public List<DataItem> DList{get;}

        public V2DataList(string id, DateTime date) : base(id, date) 
        {
            DList = new List<DataItem>();
        }

        public bool Add(DataItem newItem) 
        {
            foreach(DataItem item in DList)
                if (item.Point == newItem.Point)
                    return false;
            
            DList.Add(newItem);
            return true;
        }

        public int AddDefaults(int nItems, Fv2Complex F) 
        {
            int cnt = 0;
            Random rand = new Random();
            for (int i = 0; i < nItems; i++)
            {
                Vector2 v = new Vector2((float)rand.Next(0, 10), (float)rand.Next(5, 15));
                DataItem di = new DataItem(v, F(v));
                if (Add(di))
                    cnt++;
            }
            return cnt;
        }

        public override int Count
        { get { return DList.Count; } }

        public override float MinDistance
        { 
            get {
                if(Count < 2)
                    return 0f;
                float min = Vector2.Distance(DList[0].Point, DList[1].Point);
                for (int i = 0; i < DList.Count; i++)
                    for (int j = i + 1; j < DList.Count; j++) 
                    {
                        float dist = Vector2.Distance(DList[i].Point, DList[j].Point);
                        if (dist < min)
                            min = dist;
                    }
                return min;
            }
        }

        public override string ToString() => $"Type: {(this.GetType()).ToString()} | {base.ToString()}"
            + $" | Count: {this.Count}";

        public override string ToLongString(string format)
        {
            string output = ToString() + "\nList items:";
            foreach (DataItem item in DList) output += $"\n{item.ToLongString(format)}";
            return output;
        }
    }

    class V2DataArray : V2Data
    {
        public int LabelsX{get;}
        public int LabelsY{get;}
        public Vector2 GridStep{get;}
        public Complex[,] GridValues{get;}
        
        public V2DataArray(string id, DateTime date) : base(id, date)
        {
            GridValues = new Complex[0, 0];
        }
        
        public V2DataArray(string id, DateTime date, int nx, int ny, Vector2 gridStep, Fv2Complex F) : base(id, date)
        {
            LabelsX = nx;
            LabelsY = ny;
            GridStep = gridStep;
            GridValues = new Complex[nx, ny];
            for (int i = 0; i < nx; i++)
                for (int j = 0; j < ny; j++)
                    GridValues[i, j] = F(new Vector2(gridStep.X * i, gridStep.Y * j));
        }

        public override int Count 
        { get { return LabelsX * LabelsY; } }

        public override float MinDistance
        {
            get {
                if (Count < 2)
                    return 0f;
                if (GridStep.X < GridStep.Y)
                    return GridStep.X;
                else
                    return GridStep.Y;
            }
        }
                
        public override string ToString() => $"Type: {(this.GetType()).ToString()} | {base.ToString()} |"
            + $" LabelsX: {LabelsX} | LabelsY: {LabelsY} | GridStep: {GridStep}";
        
        public override string ToLongString(string format) 
        {
            string output = ToString() + "\nArray items:";
            for (int i = 0; i < LabelsX; i++)
                for (int j = 0; j < LabelsY; j++)
                    output += $"\nPoint: {(new Vector2(GridStep.X * i, GridStep.Y * j)).ToString(format)} |"
                        + $" Value: {(GridValues[i, j]).ToString(format)}";
            return output;
        }

        public static explicit operator V2DataList(V2DataArray v2da)
        {
            V2DataList v2dl = new V2DataList(v2da.ID, v2da.Date);
            Vector2 gridStep = v2da.GridStep;
            for (int i = 0; i < v2da.LabelsX; i++)
                for (int j = 0; j < v2da.LabelsY; j++)
                    v2dl.Add(new DataItem(new Vector2(i * gridStep.X, j * gridStep.Y),
                        v2da.GridValues[i, j]));
            return v2dl;
        }
    }
    
    class V2MainCollection
    {
        private List<V2Data> V2DList;

        public V2MainCollection()
        {
            V2DList = new List<V2Data>();
        }

        public int Count
        { get { return V2DList.Count; } }

        public V2Data this[int index]
        { get { return V2DList[index]; } }

        public bool Contains(string ID)
        {
            foreach (V2Data node in V2DList) 
            {
                if (String.Equals(node.ID, ID))
                    return true;
            }
            return false;
        }

        public bool Add(V2Data v2Data)
        {
            if (Contains(v2Data.ID))
                return false;
            V2DList.Add(v2Data);
            return true;
        }

        public string ToLongString(string format) 
        {
            string output = "V2MainCollection elements:";
            foreach (V2Data node in V2DList) output += $"\n{node.ToLongString(format)}";
            return output;
        }
        
        public override string ToString()
        {
            string output = "V2MainCollection elements:";
            foreach (V2Data node in V2DList) output += $"\n{node.ToString()}";
            return output;
        }
    }
    
    static class Methods
    {
        public static Complex F1(Vector2 v2)
        { return new Complex(v2.X + v2.Y, v2.X - v2.Y); }

        public static Complex F2(Vector2 v2)
        { return new Complex(v2.X * v2.Y, 4 * v2.X); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Vector2 v = new Vector2(1, 6);
            Vector2 v2 = new Vector2(3, 4);
            Complex c = new Complex(7, 8);
            Complex c2 = new Complex(7, 5);
            DataItem di = new DataItem(v, c);
            DataItem di2 = new DataItem(v, c2);
            DataItem di3 = new DataItem(v2, c2);

            V2DataList l2 = new V2DataList("listname", DateTime.Now);
            l2.Add(di);  // added
            l2.Add(di2); // not added
            l2.Add(di3); // added

            Console.WriteLine("-----1-----");
            V2DataArray a = new V2DataArray("arrname", DateTime.Now, 2, 2, v2, Methods.F1);
            Console.WriteLine(a.ToLongString(null));
            V2DataList l = (V2DataList) a;
            Console.WriteLine(a.ToLongString(null));
            Console.WriteLine($"Array:\nCount: {a.Count} | MinDistance: {a.MinDistance}"
                + $"\nList:\nCount: {l.Count} | MinDistance: {l.MinDistance}");
            
            Console.WriteLine("\n-----2-----");
            V2MainCollection mc = new V2MainCollection();
            mc.Add(l2);
            mc.Add(a);
            Console.WriteLine(mc.ToLongString(null));
            
            Console.WriteLine("\n-----3-----");
            for (int i = 0; i < mc.Count; i++)
                Console.WriteLine($"Count: {mc[i].Count} | MinDistance: {mc[i].MinDistance}");
        }
    }
}