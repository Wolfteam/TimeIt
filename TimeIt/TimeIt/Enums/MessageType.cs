namespace TimeIt.Enums
{
    public enum MessageType
    {
        ADD_TIMER,
        EDIT_TIMER,
        ADD_INTERVAL,
        EDIT_INTERVAL,
        INTERVAL_ADDED_EDITED,
        //This are messages that are received by the main page
        MP_TOTAL_TIME_CHANGED,
        MP_START_BUTTON_IS_ENABLED,
        MP_ELAPSED_TIME_CHANGED,
        MP_REMAINING_REPETITIONS_CHANGED,
        MP_TIMER_CREATED,
        MP_TIMER_UPDATED,
        ON_NAVIGATED_BACK
    }
}
