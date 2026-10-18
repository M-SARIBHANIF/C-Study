//Console.WriteLine("Starting the application...");
//String Input = Console.ReadLine() ?? "No input provided";
//Console.WriteLine("You entered: " + Input);

//Console.WriteLine(" Enter a number: ");
//String NumberInput = Console.ReadLine() ?? "No number entered";
//Console.WriteLine("You entered: " + NumberInput);

//if(String.IsNullOrEmpty(Input))
//{
//    Console.WriteLine("Input is empty or null.");
//}
//else
//{
//    Console.WriteLine("Input is valid.");
//}

//Console.WriteLine("Counting from 1 to 10:");
//for (int i = 1; i <= 10; i++)
//{
//    Console.Write(i + " ");
//}

//// 5. Method definition
//static bool IsEven(int n)
//{
//    return n % 2 == 0;
//}


//string name = "Sarib";
//int age = 25;
//decimal salary = 150000.50m;
//double height = 1.75;
//bool isEmployed = true;
//char grade = 'A';

//Console.WriteLine($"Name: {name}");
//Console.WriteLine($"Age: {age}");
//Console.WriteLine($"Salary: {salary:N2}");     // N2 = thousands separator, 2 decimals
//Console.WriteLine($"Height: {height}m");
//Console.WriteLine($"Employed: {isEmployed}");
//Console.WriteLine($"Grade: {grade}");

//// Type conversion
//double ratio = 7 / 2;      // what do you think this prints?
//double ratio2 = 7 / 2.0;   // and this?
//Console.WriteLine($"{ratio} and {ratio2}");

//// Type info
//Console.WriteLine(age.GetType());       // System.Int32
//Console.WriteLine(salary.GetType());    // System.Decimal

// if / else if / else

//var score = 85;
//var dayNumber = 3;

//if (score >= 90) Console.WriteLine("A");
//else if (score >= 75) Console.WriteLine("B");
//else Console.WriteLine("C");

//// ternary: a one-line if/else that returns a value
//string result = score >= 50 ? "Pass" : "Fail";
//Console.WriteLine(result);

//// switch expression (modern, concise)
//string day = dayNumber switch
//{
//    1 => "Mon",
//    2 => "Tue",
//    3 => "Wed",
//    _ => "Other"       // _ is the default case
//};

//string[] names = { "Ali", "Sara", "Omar" };

//for (int i = 0; i < names.Length; i++)
//{
//    if (names[i] == "Sara") names[i] = "Not Sara";
//}

//foreach (string s in names)
//    Console.WriteLine(s);

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

// ---------- 1. Build an object graph ----------
var order = new Order
{
    Id = 101,
    Customer = new Customer { Name = "Sarib", Email = null },
    Items = new List<OrderItem>
    {
        new() { Product = "Laptop", Price = 1200m, Qty = 1 },
        new() { Product = "Mouse",  Price = 25m,   Qty = 2 }
    },
    CreatedOn = new DateTime(2026, 10, 10),
    InternalNote = "secret"
};

// ---------- 2. Serialize ----------
string json = JsonConvert.SerializeObject(order, Formatting.Indented);
Console.WriteLine("--- Serialized ---");
Console.WriteLine(json);

// ---------- 3. Deserialize ----------
Order? back = JsonConvert.DeserializeObject<Order>(json);
Console.WriteLine("--- Deserialized ---");
Console.WriteLine($"{back?.Customer?.Name} ordered {back?.Items.Count} items, total {back?.Total}");

// ---------- 4. Save to a file and load it back ----------
File.WriteAllText("order.json", json);
Order? loaded = JsonConvert.DeserializeObject<Order>(File.ReadAllText("order.json"));
Console.WriteLine($"Loaded from file: order {loaded?.Id}");

// ---------- 5. Settings: camelCase names, skip nulls ----------
var settings = new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    NullValueHandling = NullValueHandling.Ignore,
    Formatting = Formatting.Indented
};
Console.WriteLine("--- With settings ---");
Console.WriteLine(JsonConvert.SerializeObject(order, settings));

// ---------- 6. Read JSON without a class ----------
JObject obj = JObject.Parse(json);
string? customerName = (string?)obj["Customer"]?["Name"];
string? firstProduct = (string?)obj["Items"]?[0]?["Product"];
Console.WriteLine($"Without a class: {customerName}, {firstProduct}");

// ---------- 7. Bad JSON throws ----------
try
{
    JsonConvert.DeserializeObject<Order>("{ bad json");
}
catch (JsonException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// ---------- Classes go AFTER the statements ----------
class Order
{
    [JsonProperty("order_id")]          // JSON key differs from C# name
    public int Id { get; set; }

    public Customer? Customer { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public DateTime CreatedOn { get; set; }

    [JsonIgnore]                        // never written to or read from JSON
    public string InternalNote { get; set; } = "";

    public decimal Total => Items.Sum(i => i.Price * i.Qty);   // read-only: serialized, ignored on deserialize
}

class Customer
{
    public string Name { get; set; } = "";
    public string? Email { get; set; }
}

class OrderItem
{
    public string Product { get; set; } = "";
    public decimal Price { get; set; }
    public int Qty { get; set; }
}