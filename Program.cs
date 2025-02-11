///David Barlow Lab-4-Big-To-Do-List 2/4/2025
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

class TodoListApp 
{
    private TodoList _tasks;
    private bool _showHelp = true;
    private bool _insertMode = true;
    private bool _quit = false;

    public TodoListApp(TodoList tasks) {
        _tasks = tasks;
    }

    public void Run() {
        while (!_quit) {
            Console.Clear();
            Display();
            ProcessUserInput();
        }
    }

    public void Display() {
        DisplayTasks();
        if (_showHelp) {
            DisplayHelp();
        }
    }

    public void DisplayBar() {
        Console.WriteLine("----------------------------");
    }

    public string MakeRow(int i) {
        Task task = _tasks.GetTask(i);
        string arrow = "  ";
        if (task == _tasks.CurrentTask()) arrow = "->";
        string check = " ";
        if (task.Status() == Task.CompletionStatus.Finished) check = "X";
        return $"{arrow} [{check}] {task.Title}";
    }

    public void DisplayTasks() {
        DisplayBar();
        Console.WriteLine("Tasks:");
        for (int i = 0; i <_tasks.Length(); i++) {
            Console.WriteLine(MakeRow(i));
        }
        DisplayBar();
    }

    public void DisplayHelp() {
        Console.WriteLine(
@"Instructions:
   h: show/hide instructions
   ↕: select previous or next task (wrapping around at the top and bottom)
   ↔: reorder task (swap selected task with previous or next task)
   space: toggle completion of selected task
   e: edit title
   i: insert new tasks
   delete/backspace: delete task");
        DisplayBar();
    }

    private string GetTitle() {
        Console.WriteLine("Please enter task title (or [enter] for none): ");
        return Console.ReadLine()!;
    }

    public void ProcessUserInput() {
        if (_insertMode) {
            string taskTitle = GetTitle();
            if (taskTitle.Length == 0) {
                _insertMode = false;
            } else {
                _tasks.Insert(taskTitle);
            }
        } else {
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    _quit = true;
                    break;
                case ConsoleKey.UpArrow:
                    _tasks.SelectPrevious();
                    break;
                case ConsoleKey.DownArrow:
                    _tasks.SelectNext();
                    break;
                case ConsoleKey.LeftArrow:
                    _tasks.SwapWithPrevious();
                    break;
                case ConsoleKey.RightArrow:
                    _tasks.SwapWithNext();
                    break;
                case ConsoleKey.I:
                    _insertMode = true;
                    break;
                case ConsoleKey.E:
                    _tasks.CurrentTask.Title = GetTitle();
                    break;
                case ConsoleKey.H:
                    _showHelp = !_showHelp;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    _tasks.CurrentTask.ToggleStatus();
                    break;
                case ConsoleKey.Delete:
                case ConsoleKey.Backspace:
                    _tasks.DeleteSelected();
                    break;
                default:
                    break;
            }
        }
    }
}


public class TodoList
{   private List<Task> _tasks = new List<Task>();
    private int _selectIndex {get; set;}

    public void SwapTasksAt(int i, int j)
    {
        //gets the task by the number in the list
        //then it needs set to a Task varible so it is easier to swap 
        Task one = GetTask(i);
        Task two = GetTask(j);
        //the task of where i is from the index in the list gets swapped with the j and vise versa.
        _tasks[i]= two;
        _tasks[j] = one;
    }
    public int WrapperIndex(int index)
    {
        ///if nextIndex is greater than the highest index return the first index of the list
        ///if previouse index is less than the lowest index return the last index of the list
        ///Could be way more simple by just using index and if index is goes out of the bounds of the list loop it
        ///There is more how much is the index out of bounds each way?
        ///We need to find that out and return the index that corresponds to that correct spot.
        ///For this project you will only be stepping by 1 so the given code should work
        if(index < _tasks.IndexOf(_tasks[0]))///if index is less than the first index of the list
        {
            return _tasks.IndexOf(_tasks.Last());///retun the last index of the list
        }

        if(index > _tasks.IndexOf(_tasks.Last()))/// if the index is greater than the last index of the array
        {
            return _tasks.IndexOf(_tasks[0]);///return the first index of the list
        }
        /// if none of the cases are true it should just retun index
        return index;
    }

    public int previousIndex()
    {
        ///if field select index is some number subract one from that number
        ///then return that number and that should be the previous index.
        int getIndex = _selectIndex -1;
        return getIndex;
    }

    public int NextIndex()
    {
        ///if the field select indes is some number add one to that number
        ///then return that number that should be the next Index.
        int getIndex = _selectIndex +1;
        return getIndex;
    }
    public void SelectPrevious()
    {
        //gets the previouse task by index
        GetTask(previousIndex());
    }
    public void SelectNext()
    {
        ///gets the next task by index
        GetTask(NextIndex());
    }
    public void Insert(string title)
    {
        ///should insert at the nextIndex
        ///The insert method of a list adds the item into the given int index and should bump the rest of the items down one
        ///making the selected index be where it adds the new item makes the most sense
        Task newTask = new Task(title);
        _tasks.Insert(_selectIndex,newTask);
    }

    public void UpdateSelectedTitle(string title)
    {
        //get the task that is corresponds to the selected index 
        //that is the selected Task then take that task and change its Title
        Task selectedTask = GetTask(_selectIndex);
        selectedTask.SetTitle(title);

    }

    public int Length()
    {
        ///just finds the length of the array _tasks and returns it.
        ///this is just the count of the array.
        ///It starts at one and and counts up.
        ///unlikely but could lead to problems because lists start there indexing at 0.
        ///If so just make a for loop that returns the max index not Count
        return _tasks.Count();
    }
    public void DeleteSelected()
    {
        ///removes the selected task in _tasks with the _selectedIndex
        _tasks.Remove(_tasks[_selectIndex]);
    }
    public Task CurrentTask()
    {
        ///gets the task corresponding to the _selectedIndex and returns that task

        Task current = GetTask(_selectIndex);
        return current;
    }

    public Task GetTask(int index)
    {
        ///GetTask main ideas is to get the task out of the list that corresponds to the index you give it
        ///If you give a index that is out of the bounds of the list then you it should return a unkown task
        ///The Def default task is just there so the method can run.
        Task Def = new Task("Default");

        for(int i = 0; i<_tasks.Count(); i++)
        {
            if(i == index)
            {
                return _tasks[i];
            }
        }

        if(index < Length() || index > Length())
        {
            Console.WriteLine("Cannont get task Index was out of bounds");
            Task unkown = new Task("unkown");
            return unkown;
        }
        return Def;
    }
}


public class Task
{
    private string? _title{get; set;}
    private CompletionStatus _status;

    public Task(string? title)
    {
        SetTitle(title);
        _status = CompletionStatus.NotDone;
    }

    public string? Title()
    {
        ///used for reading out the title of a task
        return _title;
    }

    public void SetTitle(string? title2)
    {
        //used for setting the title of a task
        _title = title2;
    }

    public CompletionStatus Status()
    {
        ///used for reading out the status
        return _status;
    }

    public void ToggleStatus()
    {
        ///used for changing the state of a tasks
        ///pretty simple if the task is not done it changes it to done
        ///if the task is done it changes it to not done
    
        if(_status == CompletionStatus.NotDone)
        {
            _status = CompletionStatus.Finished;
        }else
        {
            _status = CompletionStatus.NotDone;
        }
    }
    public enum CompletionStatus{NotDone,Finished}    
}

  class Program {
    static void Main() {

        Task job = new Task("job");
        Debug.Assert(job.Title() == "job");
        job.SetTitle("newJob");
        Debug.Assert(job.Title() == "newJob");
        Debug.Assert(job.Status() == Task.CompletionStatus.NotDone);
        job.ToggleStatus();
        Debug.Assert(job.Status()== Task.CompletionStatus.Finished);
/*
        TodoList Do = new TodoList();
        Do.Insert("help");
        Do.Insert("jump");
        Do.Insert("sleep");
        Do.Insert("swim");
        for(int i = 0; i<Do.Length(); i++)
        {
            Console.WriteLine(Do.GetTask(i).Title());
        }

        Console.WriteLine();

        Do.SwapTasksAt(2,0);
        for(int i = 0; i<Do.Length(); i++)
        {
            Console.WriteLine(Do.GetTask(i).Title());
        }
        Console.WriteLine();
        //using 4 because that is how many items I put in using the insert command.
        Debug.Assert(Do.Length() == 4);
        Do.Insert("fly");

        for(int i = 0; i<Do.Length(); i++)
        {
            Console.WriteLine(Do.GetTask(i).Title());
        }

*/
        new TodoListApp(new TodoList()).Run();
    }
  }