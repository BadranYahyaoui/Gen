public class TeacherFindRequest : FindRequest<Teacher> {

 
  [JsonProperty("teachername")]
  public  string  TeacherName { get; set; }
  [JsonProperty("teacheradress")]
  public  string  TeacherAdress { get; set; }

}
