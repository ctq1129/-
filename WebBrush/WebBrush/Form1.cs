using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using motorStruct;
using System.IO;

namespace WebBrush
{
    public partial class Form1 : Form
    {
        motorReader mr;
        HtmlDocument myhd;
        webOperation wo;
        choosePhotos pictureList ;
        choosePhotos pictureListCopy;
        int loopStep=1;
        int intHaveBrushNum = 0;
        bool webLoad = false;
        bool clickOrChoosePicture = false;
        int haveChoosePicture = 0;
        bool stopPageChanged = false;
        bool loopStart = false;
        bool boolFirstStart=true;
        Random rd = new Random();
        int picture;
        #region     数据，调试准备

        //数据预加载
        public Form1()
        {

            mr = new motorReader();
            InitializeComponent();
        }
         
        //对错误进行处理
        //数据初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Motor motor in mr.myMotorList)
            {
                this.comboBox2.Items.Add(motor.MainName);
            }
            string password = "jinjiaelagr23669";
            progressBar1.Maximum =6;//进度条最大值初始化
            Clipboard.SetDataObject(password, true);
        }
        #endregion
        #region 窗口1功能群
        //选择型号

        //无限检测定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            monitor();
        }
        private void selectPicture_Tick(object sender, EventArgs e)
        {
            if (haveChoosePicture > 4)
            {
                selectPicture.Enabled = false;
                timer1.Enabled = true;
                loopStep++;
                return;
            }
            retrievalPicture();
        }
        //打开数据调控窗
        private void button3_Click(object sender, EventArgs e)
        {
            this.progressBar1.Value = 0;
            loopStart = !loopStart;
            if (loopStart)
            {
                if (webLoad == true)
                {
                    if (wo.monitorButton("A", "添加更多图片") == 1)
                    {
                        if (comboBox2.SelectedItem != null)
                        {
                            button3.BackColor = Color.FromArgb(255, 255, 0, 0); 
                            button3.Text = "暂停";
                            timer1.Enabled = true;
                            groupBox2.Visible = false;
                            //webBrowser1.Visible = false;
                            label5.Text = comboBox2.SelectedItem.ToString();
                        }
                        else
                            MessageBox.Show("请先选择你要刷的产品");
                    }
                    else
                        MessageBox.Show("请先打开信息模板");
                }
                else
                    MessageBox.Show("请先点击打开网站");
            }
            else
                resetLoop();
        }
        //打开阿里巴巴网站
        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://offer.1688.com/offer/post/fill_product_info.htm");
            webLoad = true;
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            myhd = webBrowser1.Document;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Document.Window.ScrollTo(0, webBrowser1.Document.Window.Size.Height);//滚动条自动向下滑动
            wo = new webOperation(myhd);
        }
        public int monitorWeb()//检测网站有无加载完成
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete && webBrowser1.IsBusy == false)
            {
                return 1;
            }
            else
                return 0;
        }

        private void 调整数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void 配置环境ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //true是64位，分别修改注册表
            bool type;
            string regeditPath;
            type = Environment.Is64BitOperatingSystem;
            try
            {
                if (type)
                    regeditPath = "software\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                else
                    regeditPath = "software\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION";

                string strFullPath = Application.ExecutablePath;
                string strFileName = System.IO.Path.GetFileName(strFullPath);
                string fileName = strFileName.Substring(0, strFileName.Length - 4) + ".exe";//获取文件名字
                Microsoft.Win32.RegistryKey key = Registry.LocalMachine;
                RegistryKey software = key.OpenSubKey(regeditPath, true); //该项必须已存在
                software.SetValue(fileName, "11001", RegistryValueKind.DWord);
                MessageBox.Show("注册表修改成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "请用管理员身份打开软件");
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            boolFirstStart = true;                          //重置第一次刷这个型号    
            pictureList = new choosePhotos();       //重置图库  
        }
        #endregion
        private void resetLoop()//暂停重置
        {
            this.progressBar1.Value = 0;
            button3.Text = "开始";
            loopStep = 1;
            clickOrChoosePicture = false;
            haveChoosePicture = 0;
            stopPageChanged = false;
            webBrowser1.Navigate("https://offer.1688.com/offer/post/fill_product_info.htm");
            webBrowser1.Visible = true;
            groupBox2.Visible = false;
            selectPicture.Enabled = false;
            timer1.Enabled = false;
            intHaveBrushNum = 0;
            label6.Text = intHaveBrushNum.ToString() ;
        }
        //输入密码
 
        private void initSelectPicture()
        {
            if (!boolFirstStart)
            {
                pictureListCopy = new choosePhotos();
                pictureListCopy.ConnectTwoTable(pictureList);
            }
            label1.Text += "开始选图片";
            stopPageChanged = false;
            clickOrChoosePicture = false;
            timer1.Enabled = false;
            selectPicture.Enabled = true;
        }
        //随机选择五个图片
        private void clickPicture(string pageOfPicture,string nameOfPicture)
        {
                if (!wo.monitorPicture(nameOfPicture))
                {
                    if (!clickOrChoosePicture)
                        wo.clickButton("A", pageOfPicture);
                    else
                    {
                        haveChoosePicture++;
                        wo.clickButton("A", "title", nameOfPicture);
                        pictureListCopy.delete(picture);
                        picture = rd.Next(pictureListCopy.GetLength());
                }
                    clickOrChoosePicture = !clickOrChoosePicture;
                }
                else
                {
                    wo.clickButton("IMG", "alt", nameOfPicture);
                    pictureListCopy.delete(picture);
                    picture = rd.Next(pictureListCopy.GetLength());
                }
        }
        //遍历所有图片
        private void retrievalPicture()
        {
            selectPicture.Enabled = false;
            if(!boolFirstStart)
            {
                clickPicture(pictureListCopy.GetElement(picture).strPageOfImg, pictureListCopy.GetElement(picture).strNameOfImg);
            }
            else
            {
                if (monitorWeb() == 1)
                {
                    if (wo.monitorButton("A", "className", "next-disabled") != 1 && !stopPageChanged)
                    {
                        choosePhotos cp = wo.rememberPhotos(comboBox2.SelectedItem.ToString());
                        pictureList.ConnectTwoTable(cp);
                        wo.clickButton("A", "下一页");
                        picture = rd.Next(pictureList.GetLength());
                    }
                    else
                    {
                        pictureList.ConnectTwoTable(wo.rememberPhotos(comboBox2.SelectedItem.ToString()));
                        pictureListCopy = new choosePhotos();
                        pictureListCopy.ConnectTwoTable(pictureList);
                        stopPageChanged = true;
                        boolFirstStart = false;
                    }
                }
            }
            selectPicture.Enabled = true;
        }
        private void monitor()
        {
            timer1.Enabled = false;
            //label1.Text += loopStep;
            int nextStepAlready=0;
            switch (loopStep)
            {
                case 1:
                    nextStepAlready = monitorWeb();
                    break;
                case 2:
                    nextStepAlready = wo.monitorButton("A", "添加更多图片");
                    break;
                case 3:
                    label1.Text += "图片检测中";
                    nextStepAlready = wo.monitorButton("A", "title", "BM11");
                    break;
                case 4:
                    if (haveChoosePicture >= 5)
                    {
                        nextStepAlready = 1;
                        haveChoosePicture = 0;
                        selectPicture.Enabled =false;
                    }
                    break;
                case 5:
                    nextStepAlready = wo.monitorButton("IMG", "data-poorcode", "0");
                    break;
                default:
                    nextStepAlready = wo.monitorButton("A","管理销售中的产品");
                    break;
            }
            if (nextStepAlready == 1)
            {

                progressBar1.Value++;
                loop();
            }
            else
                timer1.Enabled = true;
        }
        #region 阿里巴巴流水线
        //private void wrongOut()
        //{
        //    StreamWriter sw = File.AppendText(@"..\..\text.txt");
        //    string w = label1.Text;
        //    sw.Write(w+"\n");
        //    sw.Close();
        //}
        private void loop()
        {
            //wrongOut();
            //label1.Text += "("+ loopStep +")";
            TitleAssemble ta = new TitleAssemble(comboBox2.SelectedIndex);
            int clickAlready = 0;
            switch (loopStep)
            {
                case 1:
                    clickAlready=wo.insertInput(ta.completeName, "subject");
                    label1.Text += "1";
                    break;
                case 2:
                    clickAlready = wo.clickButton("A", "添加更多图片");
                    label1.Text += "2";
                    break;
                case 3:
                    label1.Text += "3";
                    if (haveChoosePicture < 5)
                        initSelectPicture();
                    else
                    {
                        clickAlready = 1;
                    }
                    break;
                case 4:
                    clickAlready = wo.clickButton("EM", "插入图片");
                    break;
                case 5:
                    clickAlready = wo.clickButton("A", "同意协议条款，我要发布");
                break;
                default:
                    webBrowser1.GoBack();
                    intHaveBrushNum++;
                    label6.Text = intHaveBrushNum.ToString();
                    clickAlready = 1;
                    loopStep = 0;
                    this.progressBar1.Value = 0;
                 break;
            }
            if (clickAlready == 1)
            {
                loopStep++;
                timer1.Enabled = true;
            }
        }

        #endregion

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form3().Show();
        }
    }
    #region 网页操作类
    class webOperation
    {
        private HtmlDocument myHd;
        public webOperation(HtmlDocument hd)
        {
            myHd = hd;
        }
        public int insertInput(string insertString,string insertPlace)
        {
            HtmlElement uid = GetElement_Id(insertPlace);
            uid.SetAttribute("value", insertString);
            return 1;
        }
        public bool monitorPicture(string PictureName)
        {
            for (int i = 0; i < myHd.All.Count; i++)          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == "DIV")           //如果这个元素是A
                {
                    HtmlElement myelement = myHd.All[i];       //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.GetAttribute("title") == PictureName)       //如果这个元素的文字是
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //如果是超链接
        public int monitorButton(string tagName, string stringOuterText)
        {
            for (int i = 0; i < myHd.All.Count; i++)                          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == tagName)                     //如果这个元素是A
                {
                    HtmlElement myelement = myHd.All[i];                //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.OuterText == stringOuterText)       //如果这个元素的文字是
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
        public string monitorPageOfPicture()
        {
            for (int i = 0; i < myHd.All.Count; i++)                          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == "A")                     //如果这个元素是A
                {
                    HtmlElement myelement = myHd.All[i];                //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.GetAttribute("className") == "current")       //如果这个元素的文字是
                    {
                        return myelement.OuterText;
                    }
                }
            }
            return "";
        }
        public int clickButton(string tagName,string stringOuterText)
        {
                for (int i = 0; i < myHd.All.Count; i++)          //循环查找这个对象的每一个元素
                {
                    if (myHd.All[i].TagName == tagName)           //如果这个元素是A
                    {
                        HtmlElement myelement = myHd.All[i];       //就把这个元素实例化成一个HtmlElement对象
                        if (myelement.OuterText == stringOuterText)       //如果这个元素的文字是
                        {
                        myelement.InvokeMember("click");    //对这个元素进行点击
                        return 1;
                        }
                    }
                }
            return 0;
        }
        public HtmlElement GetElement_Id(string id)
        {
            HtmlElement e = myHd.GetElementById(id);
            return e;
        }
        //如果是其他
        public int monitorButton(string stringTagName, string stringClickPlace, string wordOfPlace)//标签，属性，字段
        {
            for (int i = 0; i < myHd.All.Count; i++)          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == stringTagName)           //如果这个元素是
                {
                    HtmlElement myelement = myHd.All[i];       //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.GetAttribute(stringClickPlace) == wordOfPlace)       //如果这个元素的文字是
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
        public int clickButton(string stringTagName,string stringClickPlace,string wordOfPlace)//标签，属性，字段
        {
            for (int i = 0; i < myHd.All.Count; i++)          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == stringTagName)           //如果这个元素是
                {
                    HtmlElement myelement = myHd.All[i];       //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.GetAttribute(stringClickPlace) == wordOfPlace)       //如果这个元素的文字是
                    {
                        myelement.InvokeMember("click");    //对这个元素进行点击
                        return 1;
                    }
                }
            }
            return 0;
        }
        public choosePhotos rememberPhotos(string type)
        {
            choosePhotos photoList = new choosePhotos();
            string page="";
            string name="";
            //常规
            for (int i = 0; i < myHd.All.Count; i++)          //循环查找这个对象的每一个元素
            {
                if (myHd.All[i].TagName == "A")           //如果这个元素是
                {
                    #region 检测当前页和检测是否有这个名字
                    HtmlElement myelement = myHd.All[i];       //就把这个元素实例化成一个HtmlElement对象
                    if (myelement.GetAttribute("title").Contains(type)||myelement.GetAttribute("title").Contains(type.ToLower()))       //如果这个元素的文字是
                    {
                        name = myelement.GetAttribute("title");
                        page =monitorPageOfPicture();
                            photoList.addData(page, name);
                        continue;
                    }
                    #endregion
                }
            }
            return photoList;
        }
    }
    #endregion
    class TitleAssemble
    {
        #region 数据
        private int structure = 3;
        private int type;
        public string completeName;
        private string[] spliteTitle = new string[4];
        private motorReader mr;
        private recommendReader rr;
        private Random ro;
        private Random rd;
        private Motor assembleMotor;
        private attributeMotorList nnMotor;
        private attributeMotorList mMotor;
        private attributeMotorList aMotor;
        #endregion
        #region 方法
        public TitleAssemble(int typeOfMotor)
        {
            type = typeOfMotor;
            mr = new motorReader();
            rr = new recommendReader();
            assembleMotor = mr.myMotorList[type];
            nnMotor = assembleMotor.NickName;
            mMotor = assembleMotor.Model;
            aMotor = assembleMotor.Application;
            assembleName();//直接组装
        }
        private bool compareApp(ref string str1, ref string str2)
        {
            if (str1 == str2)
            {
                str2 = aMotor.GetElement(rd.Next(aMotor.GetLength()));
                return compareApp(ref str1, ref str2);
            }
            else
                return true;
        }
        private void randomStructure()
        {
            int seed = Guid.NewGuid().GetHashCode();
            string app1;
            string app2;
            string strAdj1 = "";
            string strAdj2 = "";
            ro = new Random(seed);
            rd = new Random();
            //装配
            if (rd.Next(2) == 1)
                strAdj1 = "用于";
            else
                strAdj2 = "专用马达";
            app1 = aMotor.GetElement(ro.Next(aMotor.GetLength()));
            app2 = aMotor.GetElement(rd.Next(aMotor.GetLength()));
            compareApp(ref app1, ref app2);
            //加入
            spliteTitle[0] = nnMotor.GetElement(ro.Next(nnMotor.GetLength())) + "-" + mMotor.GetElement(ro.Next(mMotor.GetLength())) + "摆线液压马达";
            spliteTitle[1] = strAdj1 + app1 + "," + app2 + strAdj2;
            spliteTitle[2] = rr.recommendList[ro.Next(rr.recommendList.Count())].data;
        }
        private void shuffleTheCards()
        {
            randomStructure();
            int roll;
            string median;
            for (int i = 0; i < structure; i++)
            {
                ro = new Random(Guid.NewGuid().GetHashCode());
                roll = ro.Next(structure);
                median = spliteTitle[i];
                spliteTitle[i] = spliteTitle[roll];
                spliteTitle[roll] = median;
            }
        }
        private void assembleName()
        {
            shuffleTheCards();
            for (int i = 0; i < structure; i++)
            {
                completeName += spliteTitle[i] + ",";
            }
            quantitydetection(ref completeName);
        }
        //字符串长度检测
        private bool quantitydetection(ref string detectionString)
        {
            int bytesOfString = Encoding.Default.GetBytes(detectionString).Length;
            if (bytesOfString <= 50)//字符串检测
            {
                detectionString += "山东济宁市";
                return quantitydetection(ref detectionString);
            }
            else if (bytesOfString > 60)
            {
                detectionString = detectionString.Substring(0, (bytesOfString - 60) / 2);
                return quantitydetection(ref detectionString);
            }
            else
                return true;
        }
        #endregion
    }
    struct PhotoData
    {
        public string strPageOfImg;
        public string strNameOfImg;
    }
    class choosePhotos
    {
        private int intMaxSize = 200;
        private PhotoData[] tItems;//使用数组盛放元素
        private int intPointerLast;//始终指向最后一个元素的位置
        public choosePhotos()
        {
            this.tItems = new PhotoData[intMaxSize];
            this.intPointerLast = -1;//初始值设为-1，此时数组中元素个数为0
        }
        public PhotoData this[int i]//索引器方便返回
        {
            get { return this.tItems[i]; }
        }
        //得到长度
        public int GetLength()
        {
            return this.intPointerLast + 1;
        }
        //判断是否为空
        public bool IsEmpty()
        {
            if (this.intPointerLast == -1)
                return true;
            return false;
        }

        public PhotoData GetElement(int i)
        {
            if (this.intPointerLast == -1)
            {
                MessageBox.Show("我的相册中没有这个类型产品的图片");
            }
            if (i > this.intPointerLast || i < 0)
            {
                MessageBox.Show("查找的图片超过图片列表的范围");
            }
            return this.tItems[i];
        }
        public void delete(int i)
        {
            for (int j = i; j < this.intPointerLast; j++)
            {
                this.tItems[j] = this.tItems[j + 1];
            }
            this.intPointerLast--;//表长-1
        }
        public bool IsFull()//判断是否超出容量
        {
            return this.intPointerLast + 1 == this.intMaxSize;
        }
        public void addData(string page, string name)
        {
            if (this.IsFull())//如果超出最大容量，则无法添加新元素
            {
                MessageBox.Show("相册中该类型图片过多");
            }
            else
            {
                this.intPointerLast++;
                this.tItems[this.intPointerLast].strPageOfImg = page;
                this.tItems[this.intPointerLast].strNameOfImg = name;//表长+1
            }
        }
        public void ConnectTwoTable(choosePhotos otherTable)
        {
            for (int i=0;i< otherTable.GetLength();i++)
            {
                string page= otherTable.GetElement(i).strPageOfImg;
                string name = otherTable.GetElement(i).strNameOfImg;
                if (name!="") 
                addData(page, name);
            }
        }
    }
}
