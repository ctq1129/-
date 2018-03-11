using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace motorStruct
{
    public interface IListDS<String>
    {
        int GetLength();
        String GetElement(int i);
    }
    class attributeMotorList:IListDS<String>
    {
        private int intMaxSize=50;//最大容量事先确定，使用数组必须先确定容量
        private string[] tItems;//使用数组盛放元素
        private int intPointerLast;//始终指向最后一个元素的位置
        public attributeMotorList()
        {
            this.tItems = new String[intMaxSize];//在这里初始化最合理
            this.intPointerLast = -1;//初始值设为-1，此时数组中元素个数为0
        }
        public String this[int i]//索引器方便返回
        {
            get { return this.tItems[i]; }
        }
        //得到长度
        public int GetLength()
        {
            return this.intPointerLast + 1;
        }
        //判断是否为空
        /*public bool IsEmpty()
        {
            throw new NotImplementedException();
        }*/

        public string GetElement(int i)
        {
            if (this.intPointerLast == -1)
            {
                return "无";
            }
            if (i > this.intPointerLast || i < 0)
            {
                return "数据越界";
            }
            return this.tItems[i];
        }
        public bool IsFull()//判断是否超出容量
        {
            return this.intPointerLast + 1 == this.intMaxSize;
        }
        public void  addData(string data)
        {
            if (this.IsFull())//如果超出最大容量，则无法添加新元素
            {
                //throw new Exception("产品属性数据过多，越界！");
            }
            else
            {
                this.tItems[++this.intPointerLast] = data;//表长+1
            }
        }
    }
    class Recommend
    {
        public string data;
        public Recommend(){ }
        public string Data
        {
             get { return data; }
             set { data = value; }
        }
    }
    class Motor
    {
        #region 结构
        private string mainName;
        private attributeMotorList nickName;
        private attributeMotorList application;
        private attributeMotorList model;
        #endregion
        public Motor()
        {
            nickName = new attributeMotorList();
            application = new attributeMotorList();
            model = new attributeMotorList();
        }
        #region 属性
        public string MainName
        {
            get { return mainName; }
            set { mainName = value; }
        }
        public attributeMotorList NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }
        public attributeMotorList Application
        {
            get { return application; }
            set { application = value; }
        }
        public attributeMotorList Model
        {
            get { return model; }
            set { model = value; }
        }
        #endregion
    }
    class xmlReader
    {
        protected XmlDocument doc;
        protected XmlNode xn;
        public xmlReader()
        {
            doc= new XmlDocument();
            doc.Load("motorData.xml");
            xn = doc.SelectSingleNode("motordata");
        }
    }
    class recommendReader:xmlReader
    {
        public List<Recommend> recommendList = new List<Recommend>();
        public recommendReader()
        {
            XmlNode xnRd = xn.SelectSingleNode("recommend");
            XmlNodeList xnl = xnRd.ChildNodes;
            foreach(XmlNode xn1 in xnl)
            {
                Recommend rd = new Recommend();
                XmlElement xe = (XmlElement)xn1;
                rd.Data = xe.InnerText;
                recommendList.Add(rd);
            }
        }
    }
    class motorReader:xmlReader
    {
        public List<Motor> myMotorList = new List<Motor>();
        public motorReader()
        {
            XmlNode xnMt = xn.SelectSingleNode("motortitle");
            XmlNodeList xnl = xnMt.ChildNodes;
            foreach (XmlNode xn1 in xnl)
            {
                Motor myMotor = new Motor();
                XmlElement xe = (XmlElement)xn1;
                myMotor.MainName = xe.GetAttribute("mainname").ToString();
                XmlNodeList xnl0 = xe.ChildNodes;
                loadTree(xnl0,myMotor);
                myMotorList.Add(myMotor);
            }
        }
        private void loadTree(XmlNodeList xnl,Motor myMotor)
        {
            if (xnl.Item(0) ==null)
                return;
            else
            {
                foreach (XmlNode xn in xnl)
                {

                    switch(xn.Name)
                    {
                        case "app":
                            myMotor.Application.addData(xn.InnerText);
                            break;
                        case "nick":
                            myMotor.NickName.addData(xn.InnerText);
                            break;
                        case "id":
                            myMotor.Model.addData(xn.InnerText);
                            break;
                        default:
                            break;
                    }
                    XmlNodeList xnl2 = xn.ChildNodes;
                    loadTree(xnl2,myMotor);
                }
            }
        }
    }
}
