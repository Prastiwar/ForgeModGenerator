using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ForgeModGenerator.Services
{
    public class AddRemoveCommand<T, U> : TrackedCommand
    {
        public AddRemoveCommand(ICommandTracker tracker, Func<T, U> execute, Func<Tuple<T, U>, T> undoExecute) : base(tracker)
        {
            Tracker = tracker;

            ExecuteCommand = new DelegateCommand<T>((arg) => {
                LastUsedArguments = new Tuple<T, U>(arg, execute.Invoke(arg));
                Tracker.Add(this);
            });

            UndoCommand = new DelegateCommand<Tuple<T, U>>((arg) => {
                LastUsedArguments = undoExecute.Invoke(arg);
                Tracker.Add(this);
            });
        }
    }

    public class TrackedCommand<T> : TrackedCommand
    {
        public TrackedCommand(ICommandTracker tracker, Action<T> execute, Action<T> undoExecute) : base(tracker)
        {
            ExecuteCommand = new DelegateCommand<T>((arg) => {
                LastUsedArguments = arg;
                execute.Invoke(arg);
                Tracker.Add(this);
            });

            UndoCommand = new DelegateCommand<T>((arg) => {
                LastUsedArguments = arg;
                undoExecute.Invoke(arg);
                Tracker.Add(this);
            });
        }
    }

    public class TrackedCommand
    {
        public ICommandTracker Tracker { get; protected set; }
        public ICommand ExecuteCommand { get; protected set; }
        public ICommand UndoCommand { get; protected set; }

        public object LastUsedArguments { get; protected set; }

        protected TrackedCommand(ICommandTracker tracker) => Tracker = tracker;

        protected TrackedCommand(ICommandTracker tracker, ICommand executeCommand, ICommand undoCommand)
        {
            Tracker = tracker;
            ExecuteCommand = executeCommand;
            UndoCommand = UndoCommand;
        }

        public TrackedCommand(ICommandTracker tracker, Action execute, Action undoExecute) : this(tracker)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            else if (undoExecute == null)
            {
                throw new ArgumentNullException(nameof(undoExecute));
            }

            ExecuteCommand = new DelegateCommand(() => {
                execute.Invoke();
                Tracker.Add(this);
            });

            UndoCommand = new DelegateCommand(() => {
                undoExecute.Invoke();
                Tracker.Add(this);
            });
        }
    }

    public interface ICommandTracker
    {
        void Add(TrackedCommand invokeCommand);
        void Undo(int count);
        void Redo(int count);
        void Reset();
    }

    public class CommandTracker : ICommandTracker
    {
        protected Stack<TrackedCommand> UndoStack;
        protected Stack<TrackedCommand> RedoStack;

        public CommandTracker(int capacity = 5)
        {
            UndoStack = new Stack<TrackedCommand>(capacity);
            RedoStack = new Stack<TrackedCommand>(capacity);
        }

        public void Add(TrackedCommand invokeCommand)
        {
            if (invokeCommand == null)
            {
                throw new ArgumentNullException(nameof(invokeCommand));
            }
            UndoStack.Push(invokeCommand);
            RedoStack.Clear();
        }

        public bool Redo()
        {
            bool canRedo = RedoStack.Count > 0;
            if (canRedo)
            {
                TrackedCommand cmd = RedoStack.Pop();
                cmd.ExecuteCommand.Execute(cmd.LastUsedArguments);
                Add(cmd);
            }
            return canRedo;
        }

        public bool Undo()
        {
            bool canUndo = UndoStack.Count > 0;
            if (canUndo)
            {
                TrackedCommand cmd = UndoStack.Pop();
                cmd.UndoCommand.Execute(cmd.LastUsedArguments);
                RedoStack.Push(cmd);
            }
            return canUndo;
        }

        public void Redo(int count)
        {
            if (count > RedoStack.Count)
            {
                count = RedoStack.Count;
            }
            for (int i = 0; i < count; i++)
            {
                TrackedCommand cmd = RedoStack.Pop();
                cmd.ExecuteCommand.Execute(cmd.LastUsedArguments);
                Add(cmd);
            }
        }

        public void Undo(int count)
        {
            if (count > UndoStack.Count)
            {
                count = UndoStack.Count;
            }
            for (int i = 0; i < count; i++)
            {
                TrackedCommand cmd = UndoStack.Pop();
                cmd.UndoCommand.Execute(cmd.LastUsedArguments);
                RedoStack.Push(cmd);
            }
        }

        public void Reset()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}
