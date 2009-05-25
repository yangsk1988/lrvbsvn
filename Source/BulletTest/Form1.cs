using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle;
using VirtualBicycle.CollisionModel;
using VirtualBicycle.CollisionModel.Broadphase;
using VirtualBicycle.CollisionModel.Dispatch;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Graphics.Effects;
using PM = VirtualBicycle.Physics.MathLib;

namespace BulletTest
{
    public unsafe partial class Form1 : Form
    {
        CollisionDispatcher collDisp;
        DiscreteDynamicsWorld physWorld;
        SimpleBroadphase broadphase;

        RigidBody bodyA;
        RigidBody bodyB;

        struct TestVertex
        {
            public Vector3 pos;
            public float dummy;
            public Vector2 tex1;

            public static VertexFormat Format
            {
                get { return VertexFormat.PositionRhw | VertexFormat.Texture1; }
            }
        }

        Direct3D d3d;
        Device device;

        Mesh ma;
        Mesh mb;

        TestVertex[] quad;
        TestVertex[] quadsm;
        //TestVertex2[] testQuad;

        Vector3 pos;
        Vector3 lpos;


        Matrix lview;
        Matrix lproj;

        Matrix proj;
        Matrix view;




        Effect effect;
        EffectHandle paraMVP;
        EffectHandle paraTT;
        EffectHandle paraSM;
        EffectHandle paraLdir;

        Texture rt;
        Texture depthRt;


        //Texture testTex;


        EffectHandle genSMTec;
        EffectHandle dsTec;


        float angle;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            angle += 0.01f;

            device.BeginScene();

            // draw the shadow map
            Surface oldRt = device.GetRenderTarget(0);
            Surface oldDep = device.DepthStencilSurface;

            device.DepthStencilSurface = depthRt.GetSurfaceLevel(0);
            device.SetRenderTarget(0, rt.GetSurfaceLevel(0));
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, -1, 1, 0);

            effect.Technique = genSMTec;

            effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
            effect.BeginPass(0);

            DrawSM();

            effect.EndPass();
            effect.End();




            // draw normal scene
            device.SetRenderTarget(0, oldRt);
            device.DepthStencilSurface = oldDep;



            device.VertexShader = null;
            device.PixelShader = null;


            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkBlue.ToArgb(), 1, 0);
            effect.Technique = dsTec;
            effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
            effect.BeginPass(0);
            //effect.SetValue(paraSM, rt);
            effect.SetTexture(paraSM, rt);


            DrawObjects();
            effect.EndPass();
            effect.End();

            device.VertexShader = null;
            device.PixelShader = null;







            device.SetTexture(0, rt);
            device.VertexFormat = TestVertex.Format;
            device.DrawUserPrimitives<TestVertex>(PrimitiveType.TriangleStrip, 0, 2, quadsm);

            //device.SetTexture(0, null);
            //device.VertexFormat = TestVertex2.Format;
            //device.DrawUserPrimitives<TestVertex2>(PrimitiveType.LineStrip, 0, 3, testQuad);



            device.EndScene();

            device.Present();

            physWorld.StepSimulation(1.0f / 60.0f, 10, 1.0f / 60.0f);


            //bodyA.AddForce(-Vector3.UnitZ);

        }


        //Matrix GetTransform(RigidBody body)
        //{
        //    VirtualBicycle.Physics.MathLib.Matrix m;
        //    body.MotionState.GetWorldTransform(out m);

        //    Matrix res;
        //    res.M11 = m.M11;
        //    res.M12 = m.M21;
        //    res.M13 = m.M31;

        //    res.M21 = m.M12;
        //    res.M22 = m.M22;
        //    res.M23 = m.M32;

        //    res.M31 = m.M13;
        //    res.M32 = m.M23;
        //    res.M33 = m.M33;

        //    res.M14 = m.M14;
        //    res.M24 = m.M24;
        //    res.M34 = m.M34;

        //    res.M41 = m.M41;
        //    res.M42 = m.M42;
        //    res.M43 = m.M43;
        //    res.M44 = m.M44;

        //    return res;
        //}

        void DrawObjects()
        {

            //device.SetRenderState(RenderState.DepthBias, 0.1f);
            //device.SetRenderState(RenderState.SlopeScaleDepthBias, 0.1f);

            device.SetTransform(TransformState.Projection, proj);
            device.SetTransform(TransformState.View, view);


            Vector3 ld = this.lpos;
            ld = -ld;
            ld.Normalize();

            Matrix world = ((DefaultMotionState)bodyB.MotionState).GraphicsWorldTransform;

            //world *= Matrix.RotationY(angle);

            effect.SetValue(paraLdir, ld);
            effect.SetValue(paraMVP, world * view * proj);
            effect.SetValue(paraTT, world * lview * lproj);

            effect.CommitChanges();

            device.SetTransform(TransformState.World, world);
            mb.DrawSubset(0);

            //world = Matrix.Identity;
            //world *= Matrix.RotationY(angle);

            //world = ((DefaultMotionState)bodyA.MotionState).GraphicsWorldTransform;
            world = ((DefaultMotionState)bodyA.MotionState).GraphicsWorldTransform;

            effect.SetValue(paraMVP, world * view * proj);
            effect.SetValue(paraTT, world * lview * lproj);
            effect.CommitChanges();

            device.SetTransform(TransformState.World, world);
            ma.DrawSubset(0);
            //device.SetRenderState(RenderState.DepthBias, 0f);
            //device.SetRenderState(RenderState.SlopeScaleDepthBias, 0f);
        }
        void DrawSM()
        {
            device.SetTexture(0, null);


            device.SetTransform(TransformState.Projection, lproj);
            device.SetTransform(TransformState.View, lview);



            Matrix world = ((DefaultMotionState)bodyB.MotionState).GraphicsWorldTransform;

            effect.SetValue(paraMVP, world * lview * lproj);
            effect.CommitChanges();

            device.SetTransform(TransformState.World, world);
            mb.DrawSubset(0);

            //world = Matrix.Identity;
            //world *= Matrix.RotationY(angle);

            world = ((DefaultMotionState)bodyA.MotionState).GraphicsWorldTransform;

            effect.SetValue(paraMVP, world * lview * lproj);
            effect.CommitChanges();

            device.SetTransform(TransformState.World, world);


        }

        CollisionShape BuildShape()
        {
            PM.Vector3[] vertices = new PM.Vector3[8];
            vertices[0] = new Vector3(-2.5f, -0.25f, -2.5f);
            vertices[1] = new Vector3(2.5f, -0.25f, -2.5f);
            vertices[2] = new Vector3(-2.5f, -0.25f, 2.5f);
            vertices[3] = new Vector3(2.5f, -0.25f, 2.5f);

            vertices[4] = new Vector3(-2.5f, 0.5f, -2.5f);
            vertices[5] = new Vector3(2.5f, 0.5f, -2.5f);
            vertices[6] = new Vector3(-2.5f, 0.5f, 2.5f);
            vertices[7] = new Vector3(2.5f, 0.5f, 2.5f);

            int[] ibDst = new int[36];

            ibDst[0] = 0;
            ibDst[1] = 1;
            ibDst[2] = 3;

            ibDst[3] = 0;
            ibDst[4] = 3;
            ibDst[5] = 2;


            ibDst[6] = 0;
            ibDst[7] = 4;
            ibDst[8] = 5;

            ibDst[9] = 0;
            ibDst[10] = 5;
            ibDst[11] = 1;



            ibDst[12] = 2;
            ibDst[13] = 6;
            ibDst[14] = 4;

            ibDst[15] = 2;
            ibDst[16] = 4;
            ibDst[17] = 0;


            ibDst[18] = 3;
            ibDst[19] = 7;
            ibDst[20] = 6;

            ibDst[21] = 3;
            ibDst[22] = 6;
            ibDst[23] = 2;


            ibDst[24] = 1;
            ibDst[25] = 5;
            ibDst[26] = 7;

            ibDst[27] = 1;
            ibDst[28] = 7;
            ibDst[29] = 3;


            ibDst[30] = 6;
            ibDst[31] = 7;
            ibDst[32] = 5;

            ibDst[33] = 6;
            ibDst[34] = 5;
            ibDst[35] = 4;

            TriangleIndexVertexArray meshData = new TriangleIndexVertexArray(
                ibDst.Length / 3, ibDst, sizeof(int) * 3, 
                vertices.Length, vertices, sizeof(float) * 3);
            return new TriangleMeshShape(meshData);
        }
        CollisionShape BuildShape2()
        {
            PM.Vector3[] vertices = new PM.Vector3[8];
            vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, -0.5f, 0.5f);
            vertices[3] = new Vector3(0.5f, -0.5f, 0.5f);

            vertices[4] = new Vector3(-0.5f, 0.5f, -0.5f);
            vertices[5] = new Vector3(0.5f, 0.5f, -0.5f);
            vertices[6] = new Vector3(-0.5f, 0.5f, 0.5f);
            vertices[7] = new Vector3(0.5f, 0.5f, 0.5f);

            int[] ibDst = new int[36];

            ibDst[0] = 0;
            ibDst[1] = 1;
            ibDst[2] = 3;

            ibDst[3] = 0;
            ibDst[4] = 3;
            ibDst[5] = 2;


            ibDst[6] = 0;
            ibDst[7] = 4;
            ibDst[8] = 5;

            ibDst[9] = 0;
            ibDst[10] = 5;
            ibDst[11] = 1;



            ibDst[12] = 2;
            ibDst[13] = 6;
            ibDst[14] = 4;

            ibDst[15] = 2;
            ibDst[16] = 4;
            ibDst[17] = 0;


            ibDst[18] = 3;
            ibDst[19] = 7;
            ibDst[20] = 6;

            ibDst[21] = 3;
            ibDst[22] = 6;
            ibDst[23] = 2;


            ibDst[24] = 1;
            ibDst[25] = 5;
            ibDst[26] = 7;

            ibDst[27] = 1;
            ibDst[28] = 7;
            ibDst[29] = 3;


            ibDst[30] = 6;
            ibDst[31] = 7;
            ibDst[32] = 5;

            ibDst[33] = 6;
            ibDst[34] = 5;
            ibDst[35] = 4;

            TriangleIndexVertexArray meshData = new TriangleIndexVertexArray(
                ibDst.Length / 3, ibDst, sizeof(int) * 3,
                vertices.Length, vertices, sizeof(float) * 3);
            return new TriangleMeshShape(meshData);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            d3d = new Direct3D();// Direct3D.Initialize();

            PresentParameters pm = new PresentParameters();
            pm.Windowed = true;
            pm.SwapEffect = SwapEffect.Discard;
            pm.BackBufferWidth = this.ClientSize.Width;
            pm.BackBufferHeight = this.ClientSize.Height;

            //pm.BackBufferFormat = Format.D24S8;

            pm.AutoDepthStencilFormat = Format.D24S8;
            pm.EnableAutoDepthStencil = true;

            device = new Device(d3d, 0, DeviceType.Hardware, this.Handle, CreateFlags.HardwareVertexProcessing, pm);



            pos = new Vector3(3, 3, 3);
            Vector3 up = new Vector3(-1, 1, -1);
            up.Normalize();

            proj = Matrix.PerspectiveFovRH((float)(Math.PI) / 4f, (float)this.ClientSize.Width / (float)this.ClientSize.Height, 1, 1000);
            view = Matrix.LookAtRH(pos, new Vector3(), up);

            //lproj = Matrix.OrthoRH(5.12f, 5.12f, 1, 10);
            lproj = Matrix.PerspectiveFovRH((float)(Math.PI) / 4f, 1, 1, 10);

            lpos = new Vector3(3, 3, -3);
            up = new Vector3(-1, 1, 1);
            up.Normalize();
            lview = Matrix.LookAtRH(lpos, new Vector3(), up);




            ma = Mesh.CreateBox(device, 1, 1, 1);//Mesh.CreateSphere(device, 0.5f, 10, 10); 
            mb = Mesh.CreateBox(device, 5, 1, 5);

            ma.ComputeNormals();
            mb.ComputeNormals();

            Mesh tmp = ma.Clone(device, MeshFlags.Managed, VertexFormat.Normal | VertexFormat.Position | VertexFormat.Texture1);
            ma.Dispose();
            ma = tmp;

            Material matColor = new Material();
            matColor.Diffuse = new Color4(1, 1, 1, 1);
            matColor.Ambient = new Color4(1, 0, 0, 1);

            ExtendedMaterial[] mat = new ExtendedMaterial[1];
            mat[0].MaterialD3D = matColor;
            ma.SetMaterials(mat);
            mb.SetMaterials(mat);



            device.SetRenderState(RenderState.Lighting, false);

            device.SetRenderState(RenderState.ZEnable, true);
            device.SetRenderState(RenderState.ZWriteEnable, true);



            device.SetRenderState<Cull>(RenderState.CullMode, Cull.Clockwise);


            rt = new Texture(device, 512, 512, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            depthRt = new Texture(device, 512, 512, 1, Usage.DepthStencil, Format.D24X8, Pool.Default);

            //depthRt = Surface.CreateDepthStencil(device, 512, 512, Format.D16, MultisampleType.None, 0, false);

            quad = new TestVertex[4]
            {
                new TestVertex{ pos = new Vector3(  0,  0, 0), tex1 = new Vector2(0,0), dummy = 0},
                new TestVertex{ pos = new Vector3(  0,512, 0), tex1 = new Vector2(0,1), dummy = 0},
                new TestVertex{ pos = new Vector3(512,  0, 0), tex1 = new Vector2(1,0), dummy = 0},
                new TestVertex{ pos = new Vector3(512,512, 0), tex1 = new Vector2(1,1), dummy = 0}

            };
            quadsm = new TestVertex[4]
            {
                new TestVertex{ pos = new Vector3(  0,  0, 0), tex1 = new Vector2(0,0), dummy = 0},
                new TestVertex{ pos = new Vector3(  0,128, 0), tex1 = new Vector2(0,1), dummy = 0},
                new TestVertex{ pos = new Vector3(128,  0, 0), tex1 = new Vector2(1,0), dummy = 0},
                new TestVertex{ pos = new Vector3(128,128, 0), tex1 = new Vector2(1,1), dummy = 0}

            };

            //testQuad = new TestVertex2[4]
            //{
            //    new TestVertex2{ pos = new Vector3(  512,  0, 0), diffuse = -1, dummy = 0},
            //    new TestVertex2{ pos = new Vector3(  512,128, 0), diffuse = -1, dummy = 0},
            //    new TestVertex2{ pos = new Vector3(128 + 512,  0, 0), diffuse = -1, dummy = 0},
            //    new TestVertex2{ pos = new Vector3(128 + 512,128, 0), diffuse = -1, dummy = 0}
            //};

            string err;
            effect = Effect.FromFile(device, "HardwareShadowMap.fx", null, null, null, ShaderFlags.None, null, out err);


            paraMVP = new EffectHandle("mvp");
            paraSM = new EffectHandle("ShadowMap");
            paraTT = new EffectHandle("TexTransform");

            paraLdir = new EffectHandle("lightDir");

            genSMTec = new EffectHandle("GenerateShadowMap");
            dsTec = new EffectHandle("RenderScene");

            collDisp = new CollisionDispatcher();

            broadphase = new SimpleBroadphase(); //new AxisSweep3(new Vector3(-1000), new Vector3(1000), 100);
            physWorld = new DiscreteDynamicsWorld(collDisp, broadphase);

            MotionState stateA = new DefaultMotionState();
            CollisionShape shapeA = new BoxShape(new Vector3(0.5f));

            //VirtualBicycle.Physics.MathLib.Vector3 inertia;
            //shapeA.CalculateLocalInertia(10, out inertia);


            bodyA = new RigidBody(1, stateA, shapeA, new PM.Vector3(1), 0, 0, 0.5f, 0);
            bodyA.Translate(new Vector3(0, 2, 0));
            //bodyA.ApplyTorque(new Vector3(1, 0, 0));
            bodyA.ApplyCentralImpulse(new Vector3(0, 0, -2f));


            CollisionShape shapeB = BuildShape();// new BoxShape(new Vector3(2.5f, 0.5f, 2.5f));

            //Texture tex = TextureLoader.LoadDisplacementMap(device, new FileLocation(@"D:\Documents\Desktop\新建文件夹\saltlake.dmp"));
            //float[] testHiData = new float[513 * 513];
            //float* src = (float*)tex.LockRectangle(0, LockFlags.ReadOnly).Data.DataPointer.ToPointer();
            //fixed (float* dataPtr = &testHiData[0])
            //{
            //    Memory.Copy(src, dataPtr, 513 * 513 * sizeof(float));
            //}
            //tex.UnlockRectangle(0);


            Matrix rot = Matrix.RotationQuaternion(Quaternion.RotationAxis(Vector3.UnitX, 45 * MathEx.PIf / 180f));

            //TriangleIndexVertexArray meshData = new TriangleIndexVertexArray(vertices, ibDst);
            //BvhTriangleMeshShape shapeB = new BvhTriangleMeshShape(meshData);
            MotionState stateB = new DefaultMotionState(rot * Matrix.Translation(0, 0, 0));
            //HeightFieldTerrainShape shapeB = new HeightFieldTerrainShape(513, 513, testHiData, 5000, 1, true, true);
            //shapeB.LocalScaling = new VirtualBicycle.Physics.MathLib.Vector3(1, 1, 1);
            
            bodyB = new RigidBody(0, stateB, shapeB);
            bodyB.CollisionFlags |= CollisionOptions.StaticObject;

            //bodyB.Translate(new Vector3(0, -2, 0));
            //bodyB.CollisionFlags |= CollisionOptions.NoContactResponse;
            //bodyB.CollisionFlags |= CollisionOptions.StaticObject;

            physWorld.AddRigidBody(bodyA);
            physWorld.AddRigidBody(bodyB);


            //Point2PointConstraint p2pconst = new Point2PointConstraint(bodyB, new Vector3(0, -2, 0));
            //physWorld.AddConstraint(p2pconst);

        }

    }
}
