using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.Drawing;

namespace ParseServerConsole
{
    /// <summary>
    /// Simple console application to demonstrate using the Parse .NET SDK
    /// Prereq: A parse-server running on localhost:1337 connected to MongoDB
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Register Parse subclasses and init the ParseClient");

            ParseObject.RegisterSubclass<Hello>();
            ParseObject.RegisterSubclass<World>();
            ParseClient.Initialize(new ParseClient.Configuration {
                ApplicationId = "myAppId",
                Server="http://localhost:1337/parse/"
            });

            Console.WriteLine("Generate an image to save on parse-server.");
            Bitmap flag = new Bitmap(10, 10);
            for (int x = 0; x < flag.Height; x++)
                for (int y = 0; y < flag.Width; y++)
                    flag.SetPixel(x, y, Color.White);

            for (int x = 0; x < flag.Height; x++)
                flag.SetPixel(x,x, Color.Red);

            // Convert the new bitmap image to byte array for ParseFile
            ImageConverter converter = new ImageConverter();
            byte[] bytes = (byte[])converter.ConvertTo(flag, typeof(byte[]));
            Parse.ParseFile flagImage = new ParseFile("flag.bmp", bytes);

            Console.WriteLine("Saving the image to parse-server.");
            Task fileContinuationTask = flagImage.SaveAsync().ContinueWith((antecedent) => {
                Console.WriteLine("Finished saving file to parse-server");
            });
            fileContinuationTask.Wait();

            Console.WriteLine("Generate data to save on parse-server using subclassing");

            World earth = new World
            {
                Name = "Earth",
                Message = "Hello from Earth!",
                Flag = flagImage
            };

            World mars = new World
            {
                Name = "Mars",
                Message = "Hello from Mars!",
                Flag = flagImage
            };
            World saturn = new World
            {
                Name = "Saturn",
                Message = "Hello from Saturn!",
                Flag = flagImage
            };
            World jupiter = new World
            {
                Name = "Jupiter",
                Message = "Hello from Jupiter!",
                Flag = flagImage
            };

            List<World> neighbors = new List<World>();
            neighbors.Add(mars);
            neighbors.Add(saturn);
            neighbors.Add(jupiter);

            Hello hello = new Hello {
                World = earth,
                Neighbors = neighbors
            };

            Console.WriteLine("Save subclass data to parse-server");
            Task continuationTask = hello.SaveAsync().ContinueWith((antecedent) => {
                Console.WriteLine("Save Finished! Status:{0}", antecedent.Status.ToString());
            });
            continuationTask.Wait();

            Console.WriteLine("Generate data to save on parse-server using ParseObject.Create");
            var testObject = ParseObject.Create("World"); // Since World is subclassed use .Create()
            testObject["name"] = "Venus";
            testObject["message"] = "Hello from Venus!";
            testObject["flag"] = flagImage;
            Console.WriteLine("Save object data to parse-server");
            Task contTask = testObject.SaveAsync().ContinueWith((antecedent) => {
                Console.WriteLine("Save TestObject Finished! Status:{0}", antecedent.Status.ToString());
            });
            contTask.Wait();

            Console.ReadLine();
        }
    }

    [ParseClassName("Hello")]
    public class Hello : ParseObject
    {
        [ParseFieldName("world")]
        public World World
        {
            get { return GetProperty<World>(); }
            set { SetProperty(value); }
        }
        [ParseFieldName("neighbors")]
        public IEnumerable<World> Neighbors
        {
            get { return GetProperty<IEnumerable<World>>(); }
            set { SetProperty(value); }
        }
    }

    [ParseClassName("World")]
    public class World : ParseObject
    {
        [ParseFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        [ParseFieldName("message")]
        public string Message
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        [ParseFieldName("flag")]
        public ParseFile Flag {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty(value); }
        }
    }
}
