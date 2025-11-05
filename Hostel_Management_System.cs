using System;
using System.Collections.Generic;
using System.Linq;

// --- MAIN PROGRAM ---
// This is where the application starts.
public class Program
{
    // Our 'database' service
    private static HostelService hostelService = new HostelService();

    public static void Main(string[] args)
    {
        Console.WriteLine("--- University Hostel Management System (Console) ---");
        
        // Pre-load some data to make it interesting
        hostelService.InitializeData();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. View All Students");
            Console.WriteLine("2. View All Rooms");
            Console.WriteLine("3. Add New Student");
            Console.WriteLine("4. Add New Room");
            Console.WriteLine("5. Assign Student to Room");
            Console.WriteLine("6. Create Complaint");
            Console.WriteLine("7. View All Complaints");
            Console.WriteLine("8. Exit");
            Console.Write("Enter your choice (1-8): ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllStudents();
                    break;
                case "2":
                    ViewAllRooms();
                    break;
                case "3":
                    AddNewStudent();
                    break;
                case "4":
                    AddNewRoom();
                    break;
                case "5":
                    AssignStudentToRoom();
                    break;
                case "6":
                    CreateComplaint();
                    break;
                case "7":
                    ViewAllComplaints();
                    break;
                case "8":
                    running = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static void ViewAllStudents()
    {
        Console.WriteLine("\n--- Student Roster ---");
        var students = hostelService.GetAllStudents();
        if (!students.Any())
        {
            Console.WriteLine("No students found.");
            return;
        }

        foreach (var s in students)
        {
            string room = s.Room?.RoomNumber ?? "N/A";
            Console.WriteLine($"ID: {s.StudentId}, Name: {s.Name}, Fee: {s.FeeStatus}, Room: {room}");
        }
    }

    private static void ViewAllRooms()
    {
        Console.WriteLine("\n--- Room List ---");
        var rooms = hostelService.GetAllRooms();
        if (!rooms.Any())
        {
            Console.WriteLine("No rooms found.");
            return;
        }

        foreach (var r in rooms)
        {
            int occupantCount = r.Students.Count;
            string status = occupantCount >= r.Capacity ? "Full" : "Available";
            Console.WriteLine($"Room: {r.RoomNumber}, Capacity: {occupantCount}/{r.Capacity} ({status})");
        }
    }

    private static void AddNewStudent()
    {
        Console.WriteLine("\n--- Add New Student ---");
        Console.Write("Enter Student ID (e.g., S101): ");
        string studentId = Console.ReadLine();
        Console.Write("Enter Full Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();
        Console.Write("Enter Phone: ");
        string phone = Console.ReadLine();
        Console.Write("Enter Fee Status (Paid/Unpaid): ");
        string feeStatus = Console.ReadLine();

        try
        {
            var newStudent = new Student
            {
                StudentId = studentId,
                Name = name,
                Email = email,
                Phone = phone,
                FeeStatus = feeStatus
            };
            hostelService.AddStudent(newStudent);
            Console.WriteLine("Success: Student added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void AddNewRoom()
    {
        Console.WriteLine("\n--- Add New Room ---");
        Console.Write("Enter Room Number (e.g., A-101): ");
        string roomNumber = Console.ReadLine();
        Console.Write("Enter Capacity (1, 2, 3, etc.): ");
        int capacity;
        while (!int.TryParse(Console.ReadLine(), out capacity) || capacity <= 0)
        {
            Console.Write("Invalid input. Please enter a positive number: ");
        }

        try
        {
            var newRoom = new Room
            {
                RoomNumber = roomNumber,
                Capacity = capacity
            };
            hostelService.AddRoom(newRoom);
            Console.WriteLine("Success: Room added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void AssignStudentToRoom()
    {
        Console.WriteLine("\n--- Assign Student to Room ---");
        Console.Write("Enter Student ID (e.g., S101): ");
        string studentId = Console.ReadLine();
        Console.Write("Enter Room Number (e.g., A-101): ");
        string roomNumber = Console.ReadLine();

        try
        {
            hostelService.AssignStudentToRoom(studentId, roomNumber);
            Console.WriteLine("Success: Student assigned to room!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

     private static void CreateComplaint()
    {
        Console.WriteLine("\n--- Create Complaint ---");
        Console.Write("Enter Your Student ID: ");
        string studentId = Console.ReadLine();
        Console.Write("Describe your issue: ");
        string issue = Console.ReadLine();

        try
        {
            hostelService.CreateComplaint(studentId, issue);
            Console.WriteLine("Success: Complaint submitted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ViewAllComplaints()
    {
        Console.WriteLine("\n--- Complaint Log ---");
        var complaints = hostelService.GetAllComplaints();
        if (!complaints.Any())
        {
            Console.WriteLine("No complaints found.");
            return;
        }

        foreach (var c in complaints)
        {
            Console.WriteLine($"By: {c.Student.StudentId} ({c.Student.Name}), Issue: {c.Issue}, Status: {c.Status}, Date: {c.DateSubmitted.ToShortDateString()}");
        }
    }
}

// --- DATA MODELS ---
// Based on your database_schema.md file

/// <summary>
/// Represents a student residing in the hostel.
/// </summary>
public class Student
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string FeeStatus { get; set; } // "Paid" or "Unpaid"
    
    // Navigation property
    public Room? Room { get; set; }
}

/// <summary>
/// Represents a single room in the hostel.
/// </summary>
public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }

    // Navigation property
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

/// <summary>
/// Represents a maintenance complaint filed by a student.
/// </summary>
public class Complaint
{
    public int ComplaintId { get; set; }
    public string Issue { get; set; }
    public string Status { get; set; } // "Pending", "Resolved"
    public DateTime DateSubmitted { get; set; }

    // Navigation property
    public Student Student { get; set; }
}


// --- FAKE DATABASE SERVICE ---
// This class manages all the data using in-memory Lists.
// In a real app, this class would talk to your SQL database.
public class HostelService
{
    private List<Student> students = new List<Student>();
    private List<Room> rooms = new List<Room>();
    private List<Complaint> complaints = new List<Complaint>();
    
    private int _nextStudentId = 1;
    private int _nextRoomId = 1;
    private int _nextComplaintId = 1;

    public void InitializeData()
    {
        // Add sample rooms
        var room1 = new Room { Id = _nextRoomId++, RoomNumber = "A-101", Capacity = 2 };
        var room2 = new Room { Id = _nextRoomId++, RoomNumber = "A-102", Capacity = 2 };
        var room3 = new Room { Id = _nextRoomId++, RoomNumber = "B-101", Capacity = 3 };
        rooms.AddRange(new[] { room1, room2, room3 });

        // Add sample students
        var student1 = new Student { Id = _nextStudentId++, StudentId = "S101", Name = "Rohan Sharma", Email = "rohan@test.com", Phone = "9876543210", FeeStatus = "Paid" };
        var student2 = new Student { Id = _nextStudentId++, StudentId = "S102", Name = "Priya Singh", Email = "priya@test.com", Phone = "9876543211", FeeStatus = "Unpaid" };
        students.AddRange(new[] { student1, student2 });

        // Assign one student to a room
        AssignStudentToRoom(student1.StudentId, room1.RoomNumber);
    }

    public List<Student> GetAllStudents() => students;
    public List<Room> GetAllRooms() => rooms;
    public List<Complaint> GetAllComplaints() => complaints;

    public void AddStudent(Student student)
    {
        if (students.Any(s => s.StudentId == student.StudentId))
            throw new Exception("Student ID already exists.");
        
        student.Id = _nextStudentId++;
        students.Add(student);
    }

    public void AddRoom(Room room)
    {
        if (rooms.Any(r => r.RoomNumber == room.RoomNumber))
            throw new Exception("Room Number already exists.");
        
        room.Id = _nextRoomId++;
        rooms.Add(room);
    }

    public void AssignStudentToRoom(string studentId, string roomNumber)
    {
        var student = students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null) throw new Exception("Student not found.");

        var room = rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
        if (room == null) throw new Exception("Room not found.");

        if (room.Students.Count >= room.Capacity)
            throw new Exception("Room is already full.");

        // If student was in another room, remove them first
        if (student.Room != null)
        {
            student.Room.Students.Remove(student);
        }

        // Add student to new room
        student.Room = room;
        room.Students.Add(student);
    }

    public void CreateComplaint(string studentId, string issue)
    {
        var student = students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null) throw new Exception("Student not found. Cannot file complaint.");

        var complaint = new Complaint
        {
            ComplaintId = _nextComplaintId++,
            Student = student,
            Issue = issue,
            Status = "Pending",
            DateSubmitted = DateTime.Now
        };
        complaints.Add(complaint);
    }
}


