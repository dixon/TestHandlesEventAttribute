using System;

namespace TestHandlesEventAttribute
{
    public partial class Post
    {
        public string Title { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime DeletionDate { get; set; }

        public static Post Ask()
        {
            var result = new Post { Title = "What do I need to do in order to be a programmer out at sea?" };

            Created.Fire(result);
            
            return result;
        }

        public void Edit(string newTitle)
        {
            var oldTitle = Title;
            Title = newTitle;

            Edited.Fire(this, new EditedArgs { OldTitle = oldTitle });
        }

        // these two methods use the collection to signal listeners
        public void Close(string closeReason)
        {
            ClosedDate = DateTime.UtcNow;
            Closed.Fire(this, new ClosedArgs { CloseReason = closeReason });
        }

        public void Delete()
        {
            DeletionDate = DateTime.UtcNow;
            Deleted.Fire(this);
        }
    }
}
