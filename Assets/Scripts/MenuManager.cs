using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectMenuZ
{
    /// <summary>
    /// Class for managing all <see cref="CanvasMenu"/>s.
    /// </summary>
    public static class MenuManager
    {
        /// <summary>
        /// All <see cref="CanvasMenu"/>s in the scene.
        /// </summary>
        public static List<CanvasMenu> CanvasMenus { get; set; } = new List<CanvasMenu>();
        /// <summary>
        /// All currently open <see cref="CanvasMenu"/>s in the scene.
        /// </summary>
        public static List<CanvasMenu> OpenCanvasMenus { get; set; } = new List<CanvasMenu>();

        /// <summary>
        /// Populates <see cref="CanvasMenus"/> with all <see cref="CanvasMenu"/>s in the scene.
        /// </summary>
        public static void ForcePopulate()
        {
            CanvasMenus = GameObject.FindObjectsOfType<CanvasMenu>().ToList();
            foreach (var menu in CanvasMenus)
            {
                if (menu.IsOpen)
                {
                    OpenCanvasMenus.Add(menu);
                }
            }
        }
    } 
}
