using System.Text.RegularExpressions;
using DotnetBasic;

public class Task : Entity
{
    private string status;
    private string description;
    public DateTime CreatedDate { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string Status
    {
        get
        {
            return status;     
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Status cannot be null or whitespace.");
            }

            string newStatus = value.Trim().ToLower();

            if (newStatus != "new" &&
                newStatus != "pending" &&
                newStatus != "complete")
            {
                throw new ArgumentException(
                    "Status must be 'new', 'pending', or 'complete'."
                );
            }

            status = newStatus;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Task description cannot be null or whitespace."
                );
            }

            description = value.Trim();
        }
    }

    public override void DisplayTask()
    {
        Console.WriteLine(
            $"Task Id: {Id}\n" +
            $"Task Status: {Status}\n" +
            $"Task Description: {Description}\n" +
            $"Created Date: {CreatedDate:dd/MM/yyyy HH:mm:ss}\n" +
            $"Effective Date: {EffectiveDate:dd/MM/yyyy}\n" +
            $"Updated Date: {UpdatedDate:dd/MM/yyyy HH:mm:ss}\n"
        );
    }
}

public class Program : TaskMethods
{
    private static List<Task> tasks = new List<Task>();

    // Never decreases, so IDs are never reused.
    private static int nextId = 1;

    public void AddTask()
    {
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();

        DateOnly effectiveDate;

        while (true)
        {
            Console.Write("Enter effective date (dd/MM/yyyy): ");
            string effectiveDateInput = Console.ReadLine();

            string datePattern =
                @"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$";

            if (!Regex.IsMatch(effectiveDateInput, datePattern))
            {
                Console.WriteLine(
                    "Invalid date format! Please use dd/MM/yyyy."
                );
                continue;
            }

            if (!DateOnly.TryParseExact(
                    effectiveDateInput,
                    "dd/MM/yyyy",
                    out effectiveDate))
            {
                Console.WriteLine(
                    "Invalid date! Please enter a valid calendar date."
                );
                continue;
            }

            break;
        }

        try
        {
            DateTime now = DateTime.Now;

            Task task = new Task
            {
                Id = nextId++,
                Status = "new",
                Description = description,
                CreatedDate = now,
                EffectiveDate = effectiveDate,
                UpdatedDate = now
            };

            tasks.Add(task);

            Console.WriteLine("Task added successfully!");
            Console.WriteLine($"Task ID: {task.Id}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void ListTask()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found!");
            return;
        }

        bool found = false;

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        foreach (Task task in tasks)
        {
            if (task.EffectiveDate <= today)
            {
                task.DisplayTask();
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No effective tasks found!");
        }
    }

    public Task FindById(int id)
    {
        return tasks.Find(task => task.Id == id);
    }

    public Task FindTaskById()
    {
        Console.Write("Enter task id: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid task ID!");
            return null;
        }

        Task task = FindById(id);

        if (task == null)
        {
            Console.WriteLine("Task not found!");
        }

        return task;
    }

    public void CompleteTask()
    {
        Task task = FindTaskById();

        if (task == null)
        {
            return;
        }

        task.Status = "complete";

        Console.WriteLine("Task completed successfully!");
    }

    public void DeleteTask()
    {
        Task task = FindTaskById();

        if (task == null)
        {
            return;
        }

        tasks.Remove(task);

        Console.WriteLine("Task deleted successfully!");
    }

    public void SearchTasks()
    {
        Console.Write("Enter status (new/pending/complete): ");

        string choice = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(choice))
        {
            Console.WriteLine("Status cannot be null or whitespace!");
            return;
        }

        choice = choice.Trim().ToLower();

        if (choice != "new" &&
            choice != "pending" &&
            choice != "complete")
        {
            Console.WriteLine(
                "Invalid status! Use 'new', 'pending', or 'complete'."
            );
            return;
        }

        bool found = false;

        foreach (Task task in tasks)
        {
            if (task.Status == choice)
            {
                task.DisplayTask();
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("No matching tasks found!");
        }
    }

    public static void Main()
    {
        Program program = new Program();

        bool running = true;

        while (running)
        {
            Console.WriteLine(
                "\nChoose an option:\n" +
                "1. Add a Task\n" +
                "2. List All Tasks\n" +
                "3. Complete Task by Id\n" +
                "4. Delete Task by Id\n" +
                "5. Search Task by Status\n" +
                "6. Exit"
            );

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    program.AddTask();
                    break;

                case "2":
                    program.ListTask();
                    break;

                case "3":
                    program.CompleteTask();
                    break;

                case "4":
                    program.DeleteTask();
                    break;

                case "5":
                    program.SearchTasks();
                    break;

                case "6":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option!");
                    break;
            }
        }
    }

}
