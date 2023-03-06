using static GUI.MainPage;

namespace GUI;

/// <summary>
///   Author: H. James de St. Germain
///   Date:   Spring 2023
///   
///   This code shows:
///   1) How to augment a Maui Widget (i.e., Entry) to add more information
///       a) using StyleId
///       b) using fields
///   2) How to provide a method that matches an event handler (used to clear all cells)
///   3) How to attach a method to an event handler (i.e., Completed)
/// </summary>
public class MyEntry : Entry
{
    char col = 'A';
    //JIM: better?--> int row = 0;

    /// <summary>
    ///   Function provided by "outside world" to be called whenever
    ///   this entry is modified
    /// </summary>
    private ActionOnCompleted onChange;

    /// <summary>
    ///   build an Entry element with the row "remembered"
    /// </summary>
    /// <param name="row"> unique identifier for this item </param>
    /// <param name="col"> Takes in col 
    /// <param name="changeAction"> outside action that should be invoked after this cell is modified </param>
    public MyEntry(int row, char col, ActionOnCompleted changeAction) : base()
    {
        this.StyleId = $"{row}";
        this.col = col;

        // Action to take when the user presses enter on this cell
        this.Completed += CellChangedValue;

        // "remember" outside worlds request about what to do when we change.
        onChange = changeAction;
    }

    /// <summary>
    ///   Remove focus and text from this widget
    /// </summary>
    public void ClearAndUnfocus()
    {
        this.Unfocus();
        this.Text = "";
    }

    /// <summary>
    ///   Action to take when the value of this entry widget is changed
    ///   and the Enter Key pressed.
    /// </summary>
    /// <param name="sender"> ignored, but should == this </param>
    /// <param name="e"> ignored </param>
    private void CellChangedValue(object sender, EventArgs e)
    {
        Unfocus();

        // Inform the outside world that we have changed
        onChange(col, Convert.ToInt32(this.StyleId));
    }

}
