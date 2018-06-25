using System;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Editor
{

    public interface IDrawable
    {
        void Draw();
    }

    public class EditorUIBase
    {
        /// <summary>
        /// 区域
        /// </summary>
        protected Rect m_Rect;
        public Rect rect { get { return m_Rect; } set { m_Rect = value; } }

        /// <summary>
        /// 文字描述
        /// </summary>
        protected string m_Label;
        public string label { get { return m_Label; } set { m_Label = value; } }
    }

    public class Button : EditorUIBase, IDrawable
    {
        [Serializable] public class ButtonClickedEvent : UnityEvent { }

        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public void Draw()
        {
            if (GUI.Button(m_Rect, m_Label))
            {
                m_OnClick.Invoke();
            }
        }
    }

    /// <summary>
    /// 复选框组件
    /// </summary>
    public class Toggle : EditorUIBase, IDrawable
    {
        /// <summary>
        /// 选中状态
        /// </summary>
        private bool m_IsOn;
        public bool isOn { get { return m_IsOn; } set { m_IsOn = value; } }

        /// <summary>
        ///     <para>UnityEvent callback for when a toggle is toggled.</para>
        /// </summary>
        [Serializable] public class ToggleEvent : UnityEvent<bool> { }            
        
        public Toggle.ToggleEvent onValueChanged = new ToggleEvent();

        /// <summary>
        /// 附加数据
        /// </summary>
        public object data { get; set; }

        public void Draw()
        {
            bool isOn = GUI.Toggle(m_Rect, m_IsOn, m_Label);
            bool change = isOn != this.m_IsOn;
            this.m_IsOn = isOn;
            if (this.onValueChanged != null && change)
            {
                this.onValueChanged.Invoke(this.m_IsOn);
            }
        }
    }
}
