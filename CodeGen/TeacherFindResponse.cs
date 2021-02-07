public class TeacherFindResponse : FindResponse<Teacher> {

 
  [JsonProperty("teacher")]
  public  Teacher  teacher { get; set; }

}
