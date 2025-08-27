using CareerApi.Models;

namespace TopicosP1Backend.Scripts
{
    public class DatabaseInitialization
    {
        public static void Populate(Context context) 
        {
            if (context.Careers.Any())
                return;

            var users = new List<User>
            {
                new() { Role = 'S', Login = "222008288", PasswordHash=Util.Hash("123"), Token=Util.GenToken() },
                new() { Role = 'C', Login = "admin", PasswordHash=Util.Hash("123"), Token=Util.GenToken() },
            };
            context.Users.AddRange(users);

            var careers = new List<Career>
            {
                new() { Name = "INGENIERIA INFORMATICA" },
                new() { Name = "INGENIERIA EN SISTEMAS" },
                new() { Name = "INGENIERIA EN REDES Y TELECOMUNICACIONES" },
            };
            context.Careers.AddRange(careers);
            context.SaveChanges();

            var studyplans = new List<StudyPlan>
            {
                new() { Code = "187-3", Career = careers[0] }, //INF
                new() { Code = "187-4", Career = careers[1] }, //SIS
                new() { Code = "187-5", Career = careers[2] }, //RDS
            };
            context.StudyPlans.AddRange(studyplans);
            context.SaveChanges();

            var subjects = new List<Subject>
            {
                new() { Code = "MAT101", Title = "CALCULO I" },                               //0   SEM 1 INF, SIS; RDS
                new() { Code = "INF119", Title = "ESTRUCTURAS DISCRETAS" },                   //1
                new() { Code = "INF110", Title = "INTRODUCCION A LA INFORMATICA" },           //2
                new() { Code = "FIS100", Title = "FISICA I" },                                //3
                new() { Code = "LIN100", Title = "INGLES TECNICO I" },                        //4
                new() { Code = "MAT102", Title = "CALCULO II" },                              //5   SEM 2
                new() { Code = "MAT103", Title = "ALGEBRA LINEAL" },                          //6
                new() { Code = "INF120", Title = "PROGRAMACION I" },                          //7
                new() { Code = "FIS102", Title = "FISICA II" },                               //8
                new() { Code = "LIN101", Title = "INGLES TECNICO II" },                       //9
                new() { Code = "MAT207", Title = "ECUACIONES DIFERENCIALES" },                //10  SEM 3
                new() { Code = "INF210", Title = "PROGRAMACION II" },                         //11
                new() { Code = "INF211", Title = "ARQUITECTURA DE COMPUTADORAS" },            //12
                new() { Code = "FIS200", Title = "FISICA III" },                              //13
                new() { Code = "ADM100", Title = "ADMINISTRACION" },                          //14
                new() { Code = "ELT241", Title = "TEORIA DE CAMPOS" },                        //15
                new() { Code = "RDS210", Title = "ANALISIS DE CIRCUITOS" },                   //16
                new() { Code = "MAT202", Title = "PROBABILIDADES Y ESTADIST.I" },             //17  SEM 4
                new() { Code = "MAT205", Title = "METODOS NUMERICOS" },                       //18
                new() { Code = "INF220", Title = "ESTRUCTURA DE DATOS I" },                   //19
                new() { Code = "INF221", Title = "PROGRAMACION ENSAMBLADOR" },                //20
                new() { Code = "ADM200", Title = "CONTABILIDAD" },                            //21
                new() { Code = "RDS220", Title = "ANALISIS DE CIRCUITOS ELECTRONICOS" },      //22
                new() { Code = "MAT302", Title = "PROBABILIDADES Y ESTADISTICAS II" },        //23  SEM 5
                new() { Code = "INF318", Title = "PROGRAMAC.LOGICA Y FUNCIONAL" },            //24
                new() { Code = "INF310", Title = "ESTRUCTURA DE DATOS II" },                  //25
                new() { Code = "INF312", Title = "BASE DE DATOS I" },                         //26
                new() { Code = "INF319", Title = "LENGUAJES FORMALES" },                      //27
                new() { Code = "ADM330", Title = "ORGANIZACION Y METODOS" },                  //28
                new() { Code = "ECO300", Title = "ECONOMIA PARA LA GESTION" },                //29
                new() { Code = "RDS310", Title = "ELECTRONICA APLICADA A REDES" },            //30
                new() { Code = "ELT352", Title = "SISTEMAS LOGICOS DIGITALES I" },            //31
                new() { Code = "ELT354", Title = "SEÑALES Y SISTEMAS" },                      //32
                new() { Code = "MAT329", Title = "INVESTIG. OPERATIVA I" },                   //33  SEM 6
                new() { Code = "INF342", Title = "SISTEMAS DE INFORMACION I" },               //34
                new() { Code = "INF323", Title = "SISTEMAS OPERATIVOS I" },                   //35
                new() { Code = "INF322", Title = "BASE DE DATOS II" },                        //36
                new() { Code = "INF329", Title = "COMPILADORES" },                            //37
                new() { Code = "ADM320", Title = "FINANZAS PARA LA EMPRESA" },                //38
                new() { Code = "ELT362", Title = "SISTEMAS LOGICOS DIGITALES II" },           //39
                new() { Code = "RDS320", Title = "INTERPRETAC. DE SISTEMAS Y SEÑALES" },      //40
                new() { Code = "MAT419", Title = "INVESTIGAC.OPERATIVA II" },                 //41  SEM 7
                new() { Code = "INF418", Title = "INTELIGENCIA ARTIFICIAL" },                 //42
                new() { Code = "INF413", Title = "SISTEMAS OPERATIVOS II" },                  //43
                new() { Code = "INF433", Title = "REDES I" },                                 //44
                new() { Code = "INF412", Title = "SISTEMAS DE INFORMACION II" },              //45
                new() { Code = "INF432", Title = "SIS.P/EL SOPORT.A LA TOMA DECI." },         //46
                new() { Code = "INF410", Title = "APLICACIONES CON MICROPROCESADORES" },      //47
                new() { Code = "ELT374", Title = "SISTEMAS DE COMUNICACION I" },              //48
                new() { Code = "ECO449", Title = "PREPARAC.Y EVALUAC.DE PROYECTOS" },         //49  SEM 8
                new() { Code = "INF428", Title = "SISTEMAS EXPERTOS" },                       //50
                new() { Code = "INF442", Title = "SISTEMAS DE INFORM.GEOGRAFICA" },           //51
                new() { Code = "INF423", Title = "REDES II" },                                //52
                new() { Code = "INF422", Title = "INGENIERIA DE SOFTWARE I" },                //53
                new() { Code = "INF462", Title = "AUDITORIA INFORMATICA" },                   //54
                new() { Code = "RDS421", Title = "TALLER DE AN Y DISEÑO DE REDES" },          //55
                new() { Code = "RDS429", Title = "LEGISLACION EN REDES Y COMUNICACIONES" },   //56
                new() { Code = "ELT384", Title = "SISTEMAS DE COMUNICACION II" },             //57
                new() { Code = "INF511", Title = "TALLER DE GRADO I" },                       //58  SEM 9
                new() { Code = "INF512", Title = "INGENIERIA DE SOFTWARE II" },               //59
                new() { Code = "INF513", Title = "TECNOLOGIA WEB" },                          //60
                new() { Code = "INF514", Title = "ARQUITECTURA DEL SOFTWARE" },               //61
                new() { Code = "RDS511", Title = "GESTION Y ADMINIS.DE REDES" },              //62
                new() { Code = "RDS512", Title = "REDES INALAMB. Y COMUN.MOVILES" },          //63
                new() { Code = "RDS519", Title = "SEGURID.EN REDES Y TRANSMISION DE DATOS" }, //64
                new() { Code = "GRL001", Title = "MODALIDAD DE GRADUACION" },                 //65 SEM 10
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            var prerequisites = new List<SubjectDependency>()
            {
                // SEM 1
                // INF, SIS; RDS
                new() { Prerequisite = subjects[0], Postrequisite = subjects[5], StudyPlan = studyplans[0] },
                new() { Prerequisite = subjects[1], Postrequisite = subjects[6], StudyPlan = studyplans[0] },
                new() { Prerequisite = subjects[2], Postrequisite = subjects[7], StudyPlan = studyplans[0] },
                new() { Prerequisite = subjects[3], Postrequisite = subjects[8], StudyPlan = studyplans[0] },
                new() { Prerequisite = subjects[4], Postrequisite = subjects[9], StudyPlan = studyplans[0] },
            };

            context.Prerequisites.AddRange(prerequisites);
            context.SaveChanges();

            var spsubjects = new List<SpSubject>()
            {
                new() { StudyPlan = studyplans[0], Subject = subjects[0], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[1], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[2], Credits = 5, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[3], Credits = 6, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[4], Credits = 4, Level = 1, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[5], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[6], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[7], Credits = 5, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[8], Credits = 6, Level = 2, Type = 1},
                new() { StudyPlan = studyplans[0], Subject = subjects[9], Credits = 4, Level = 2, Type = 1}
            };
            context.SpSubjects.AddRange(spsubjects);
            context.SaveChanges();
        }
    }

    public class Util
    {
        static public int Hash(String s) { return s.Select(a => (int)a).Sum(); }
        static public string GenToken() { return string.Join("", Enumerable.Repeat(0, 100).Select(n => (char)new Random().Next(32, 127))).Replace("/", "").Replace(" ", "").Replace("\\", "").Replace("\"", "").Replace("'", ""); }
    }

}
