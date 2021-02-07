public class StudentFindRequest : FindRequest<Student> {

 
  [JsonProperty("name")]
  public  string  Name { get; set; }
  [JsonProperty("grade")]
  public  double?  Grade { get; set; }
  [JsonProperty("adress")]
  public  string  Adress { get; set; }

}
