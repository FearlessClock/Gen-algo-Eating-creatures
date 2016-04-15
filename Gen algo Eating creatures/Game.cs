using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Diagnostics;

namespace Gen_algo_Eating_creatures
{
    struct Vertex
    {
        public Vector2 position;
        public Vector2 texCoord;
        public Vector4 color;

        public Color Color
        {
            get
            {
                return Color.FromArgb((int)(255 * color.W), (int)(255 * color.X), (int)(255 * color.Y), (int)(255 * color.Z));
            }
            set
            {
                this.color = new Vector4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
            }

        }
        static public int SizeInBytes
        {
            get { return Vector2.SizeInBytes * 2 + Vector4.SizeInBytes; }
        }

        public Vertex(Vector2 position, Vector2 texCoord)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.color = new Vector4(1, 1, 1, 1);
        }


    }
    class Game
    {
        public GameWindow window;
        Texture2D texture;
        Texture2D foodTex;
        Texture2D specialTex;

        Stopwatch stopWatch = new Stopwatch();

        int nmbrOfCreatures = 1;
        int lengthOfDNA = 100;
        Creature[] creatures;

        List<DrawStruct[]> drawCreatures;
        List<Food> food = new List<Food>();
        int nmbrOfFood = 0;


        GraphicsBuffer[] buf;
        long lastTime = 0;
        long timeSinceLast = 0;
        int interval = 1;
        int generationGap = 4*150;

        int year = 0;

        //Start of the vertex buffer
        GraphicsBuffer buffer = new GraphicsBuffer();

        DBConnect dBConnect = new DBConnect();
        
        public Game(GameWindow windowInput)
        {
            window = windowInput;

            window.Load += Window_Load;
            window.RenderFrame += Window_RenderFrame;
            window.UpdateFrame += Window_UpdateFrame;
            window.Closing += Window_Closing;
            Camera.SetupCamera(window, 30);
        }


        Random rand = new Random();
        private void Window_Load(object sender, EventArgs e)
        {
            texture = ContentPipe.LoadTexture("explo.bmp");
            foodTex = ContentPipe.LoadTexture("food.png");
            specialTex = ContentPipe.LoadTexture("special.png");
            creatures = new Creature[nmbrOfCreatures];

            drawCreatures = new List<DrawStruct[]>();
            

            List<string>[] parameters = dBConnect.Select("Select * from creature");

            for (int i = 0; i < nmbrOfCreatures; i++)
            {
                string dna = "";
                for (int j = 0; j < lengthOfDNA; j++)
                {
                    switch (rand.Next(0,4))
                    {
                        case 0:
                        case 1:
                            dna += 'F';
                            break;
                        case 2:
                            dna += 'L';
                            break;
                        case 3:
                            dna += 'R';
                            break;
                    }
                }
                creatures[i] = new Creature(new Vector2(window.Width/2,window.Height/2)/*(float)Math.Cos(MathHelper.DegreesToRadians(i*10))*(30+ i) +300, (float)Math.Sin(MathHelper.DegreesToRadians(i*10))* (30 + i) + 300)*/, dna/*parameters[1][i]*/, Vector2.UnitX, window.Width, window.Height, 3);
                //drawCreatures[0][i] = creatures[i].Draw();
            }

            for(int i = 0; i < nmbrOfFood; i++)
            {
                food.Add(new Food(1, new Vector3(rand.Next(0, window.Width), rand.Next(0, window.Height), 0)));
            }

            buffer.vertBuffer = new Vertex[4]
            {
                new Vertex(new Vector2(0, 0), new Vector2(0, 0)),
                new Vertex(new Vector2(0, 1), new Vector2(0, 1)),
                new Vertex(new Vector2(1, 1), new Vector2(1, 1)),
                new Vertex(new Vector2(1, 0), new Vector2(1, 0))
            };


            buffer.VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buffer.vertBuffer.Length), buffer.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            buffer.indexBuffer = new uint[4]
            {
                0,1,2,3
            };

            buffer.IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buffer.indexBuffer.Length)), buffer.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            BufferFill(buffer);

            stopWatch.Start();
        }

        private void BufferFill(GraphicsBuffer buf)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buf.vertBuffer.Length), buf.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buf.indexBuffer.Length)), buf.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < creatures.Length; i++)
            {
                dBConnect.Update(i, creatures[i].genome);
            }
        }

        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            timeSinceLast = stopWatch.ElapsedMilliseconds - lastTime;
            
            buf = Camera.CameraUpdate();
            foreach (GraphicsBuffer b in buf)
            {
                BufferFill(b);
            }
            if (interval < timeSinceLast)
            {
                drawCreatures.Add(new DrawStruct[nmbrOfCreatures]);
                for (int i = 0; i < creatures.Length; i++)
                {
                    creatures[i].Update(food);
                    drawCreatures[year][i] = creatures[i].Draw();
                }
                for (int i = 0; i < food.Count; i++)
                {
                    if (!food[i].isAlive)
                    {
                        food.RemoveAt(i);
                        food.Add(new Food(1, new Vector3(rand.Next(0, window.Width), rand.Next(0, window.Height), 0)));
                    }
                }
                lastTime = stopWatch.ElapsedMilliseconds;
                year++;
               //Console.WriteLine(year);
                //Console.ReadKey();
                if(Camera.ChangeString)// year >= generationGap)
                {
                    creatures = Evolution.Evole(creatures, window);
                    for(int i = 0; i < drawCreatures.Count; i++)
                    {
                        drawCreatures[i] = new DrawStruct[nmbrOfCreatures];
                    }
                    food.Clear();
                    for (int i = 0; i < nmbrOfFood; i++)
                    {
                        food.Add(new Food(1, new Vector3(rand.Next(0, window.Width), rand.Next(0, window.Height), 0)));
                    }
                    Console.WriteLine("Max Total: " + Evolution.lastMaxTotal);
                    Console.WriteLine("DNA: " + creatures[0].genome);
                    year = 0;
                }
            }
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            //Clear screen color
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Enable color blending, which allows transparency
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            //Blending everything for transparency
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //Create the projection matrix for the scene
            Camera.MoveCamera();

            //Bind the texture that will be used
            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            //Enable all the different arrays
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            //Load the vert and index buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VBO);
            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IBO);
            Matrix4 mat;
            for (int j = 0; j < drawCreatures.Count; j++)
            {
                for (int i = 1; i < nmbrOfCreatures; i++)
                {
                    mat = Matrix4.CreateTranslation(drawCreatures[j][i].translation);  //Create a translation matrix
                    GL.MatrixMode(MatrixMode.Modelview);    //Load the modelview matrix, last in the chain of view matrices
                    GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
                    mat = Matrix4.CreateScale(drawCreatures[j][i].scale);
                    GL.MultMatrix(ref mat);                     //Multiply the scale matrix with the modelview matrix
                    mat = Matrix4.CreateRotationZ(drawCreatures[j][i].rotation);
                    //GL.MultMatrix(ref mat);
                    GL.DrawElements(PrimitiveType.Quads, buffer.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, specialTex.ID);
            for (int j = 0; j < drawCreatures.Count; j++)
            {
                mat = Matrix4.CreateTranslation(drawCreatures[j][0].translation);  //Create a translation matrix
                GL.MatrixMode(MatrixMode.Modelview);    //Load the modelview matrix, last in the chain of view matrices
                GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
                mat = Matrix4.CreateScale(drawCreatures[j][0].scale);
                GL.MultMatrix(ref mat);                     //Multiply the scale matrix with the modelview matrix
                mat = Matrix4.CreateRotationZ(drawCreatures[j][0].rotation);
                //GL.MultMatrix(ref mat);
                GL.DrawElements(PrimitiveType.Quads, buffer.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VBO);
            //Change texture
            GL.BindTexture(TextureTarget.Texture2D, foodTex.ID);
            for (int i = 0; i < food.Count; i++)
            {
                mat = Matrix4.CreateTranslation(food[i].position);  //Create a translation matrix
                GL.MatrixMode(MatrixMode.Modelview);    //Load the modelview matrix, last in the chain of view matrices
                GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
                mat = Matrix4.CreateScale(5,5,0);
                GL.MultMatrix(ref mat);                     //Multiply the scale matrix with the modelview matrix
                GL.DrawElements(PrimitiveType.Quads, buffer.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
            }

            //Flush everything 
            GL.Flush();
            //Write the new buffer to the screen
            window.SwapBuffers();
        }
    }
}
