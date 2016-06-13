using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;

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

            Console.WriteLine("Generate data to save on parse-server");

            World earth = new World
            {
                Name = "Earth",
                Message = "Hello from Earth!"
            };

            World mars = new World
            {
                Name = "Mars",
                Message = "Hello from Mars!"
            };
            World saturn = new World
            {
                Name = "Saturn",
                Message = "Hello from Saturn!"
            };
            World jupiter = new World
            {
                Name = "Jupiter",
                Message = "Hello from Jupiter!"
            };

            List<World> neighbors = new List<World>();
            neighbors.Add(mars);
            neighbors.Add(saturn);
            neighbors.Add(jupiter);

            Hello hello = new Hello {
                World = earth,
                Neighbors = neighbors
            };

            Console.WriteLine("Save data to parse-server");

            Task continuationTask = hello.SaveAsync().ContinueWith((antecedent) => {
                Console.WriteLine("Save Finished! Status:{0}", antecedent.Status.ToString());
            });
            continuationTask.Wait();

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
    }
}
