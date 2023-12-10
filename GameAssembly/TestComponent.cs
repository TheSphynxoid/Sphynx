﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphynx;
using Sphynx.Core.Graphics;

namespace GameAssembly
{
    public class TestComponent : Component
    {
        public override void Start()
        {
            //Logger.Info($"{gameObject.ID}");
            //gameObject.AddComponent<TestComponent2>();
        }

        public override void Update()
        {
            if (Input.IsKeyDown(Keys.Up))
            {
                transform.Position += new Vector3(0, 0, -1 * Time.deltaTime);
            }
            if (Input.IsMouseButtonDown(MouseButton.LeftButton))
            {
                //var Pos = Input.GetMousePosition();
                //Logger.Info($"Mouse Position: ({Pos.x},{Pos.y})");
            }
        }
    }
}
