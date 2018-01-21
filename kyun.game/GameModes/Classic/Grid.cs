using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.UIObjs;
using Microsoft.Xna.Framework.Input;
using kyun.GameScreen;

namespace kyun.GameModes.Classic
{
    /// <summary>
    /// Logical grid for stacking
    /// </summary>
    public class Grid
    {

        float rotation = 0f;

        /// <summary>
        /// Rotation in Radians
        /// </summary>
        public float Rotation {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;

                foreach(List<IUIObject> objl in objGrid)
                    foreach (UIObjectBase obj in objl)
                        obj.AngleRotation = value;
                
            }
        }
         
        public List<List<IUIObject>> objGrid = new List<List<IUIObject>>();
        private KeyboardState kblastState;

        public Grid(int maxValue)
        {
            for (int a = 0; a < maxValue; a++)
                objGrid.Add(new List<IUIObject>());

        }

        /// <summary>
        /// Updates logic of stacking
        /// </summary>
        public void Update()
        {
            var kbstate = Keyboard.GetState();
            for (int lIndex = 0; lIndex < objGrid.Count; lIndex++)
            {
                
                List<IUIObject> objList = objGrid[lIndex];


                for (int index = objList.Count - 1; index > -1; index--){

                    Vector2 mainPosition = HitSingle.GetPositionFor(lIndex + 1);

                    objList[index].Position = new Vector2(mainPosition.X, mainPosition.Y + (index * 10));

                    var coso = objList[index];
                        ((HitSingle)coso).kbActualState = kbstate;
                        ((HitSingle)coso).kbstatelast = kblastState;
                        ((HitSingle)coso).first = (objList.IndexOf(coso) == 0) ? true : false;
                        coso.Update();
                        if (coso.Died) { objList.Remove(coso); }
                    }
            }
            kblastState = kbstate;
            
        }

        public void CleanUp()
        {
            foreach (List<IUIObject> listObj in objGrid)
                listObj.Clear();
        }

        /// <summary>
        /// Add new object in specified position
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="position">Position on the grid</param>
        /// <returns>Same Object</returns>
        public IUIObject Add(IUIObject item, int position = 0)
        {
            if (position > objGrid.Count - 1)
                throw new Exception("Position does not exist.");

            ((UIObjectBase)item).AngleRotation = rotation;

            objGrid[position].Add(item);

            return item;
        }

        internal void Render()
        {
            for (int lIndex = 0; lIndex < objGrid.Count; lIndex++)
            {
                List<IUIObject> objList = objGrid[lIndex];
                for (int index = objList.Count - 1; index > -1; index--)
                {
                    objList[index].Render();
                }
            }
        }
    }
}
