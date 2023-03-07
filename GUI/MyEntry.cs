using SS;
using static GUI.MainPage;

namespace GUI;

/// <summary>
///   Author: H. James de St. Germain, Matthew Goh, and Alex Qi
///   Date:   Spring 2023
///   
///   This code creates the entries of the spreadsheet and provides event
///   handlers for the clear events.
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
        this.Text = "";
        this.Unfocus();
        
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
