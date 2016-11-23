using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using MultiControl.Common;

namespace MultiControl
{
    class OpaqueCommand
    {
        private MyOpaqueLayer m_OpaqueLayer = null;//半透明蒙板层

        public void ShowOpaqueLayer(Control control, int alpha, int x, int y, int width, int height)
        {
            try
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new MyOpaqueLayer(alpha, false);
                    this.m_OpaqueLayer.Location = new System.Drawing.Point(x, y);
                    this.m_OpaqueLayer.Size = new System.Drawing.Size(width, height);
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.BringToFront();
                }
                else
                {
                    //this.m_OpaqueLayer.Alpha = alpha;
                    this.m_OpaqueLayer.Location = new System.Drawing.Point(x, y);
                    this.m_OpaqueLayer.Size = new System.Drawing.Size(width, height);
                    //this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }
            catch { }
        }
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="alpha">透明度</param>
        /// <param name="isShowLoadingImage">是否显示图标</param>
        public void ShowOpaqueLayer(Control control, int alpha, bool isShowLoadingImage)
        {
            try
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new MyOpaqueLayer(alpha, isShowLoadingImage);
                    this.m_OpaqueLayer.Location = new System.Drawing.Point(10, 64);
                    this.m_OpaqueLayer.Size = new System.Drawing.Size(240, 320);
                    control.Controls.Add(this.m_OpaqueLayer);
                    //this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }
            catch { }
        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        public void HideOpaqueLayer()
        {
            try
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Visible = false;
                    this.m_OpaqueLayer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                common.m_log.Add(ex.Message, LogHelper.MessageType.ERROR);
            }
        }

    }
}
