namespace Simulator.Application.Constants
{
    /// <summary>
    /// create test parameter data
    /// </summary>
    public static class TestParameterData
    {

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public static List<TestParameter> Parameters { get; } = new List<TestParameter>
        {

            new TestParameter { Parameter = "White Blood Cell Count", Abbreviation = "WBC", Unit = "cells/µL", NormalRange = "4,000–10,000 cells/µL", MinValue = 4000, MaxValue = 10000 },

            new TestParameter { Parameter = "Red Blood Cell Count", Abbreviation = "RBC", Unit = "million/µL", NormalRange = "Male: 4.7–6.1 million/µL / Female: 4.2–5.4 million/µL", MinValue = 4.2, MaxValue = 6.1 },
            

            new TestParameter { Parameter = "Hemoglobin", Abbreviation = "HGB", Unit = "g/dL", NormalRange = "Male: 14–18 g/dL / Female: 12–16 g/dL", MinValue = 12, MaxValue = 18 },
            
           
            new TestParameter { Parameter = "Hematocrit", Abbreviation = "HCT", Unit = "%", NormalRange = "Male: 42–52% / Female: 37–47%", MinValue = 37, MaxValue = 52 },
            
          
            new TestParameter { Parameter = "Platelet Count", Abbreviation = "PLT", Unit = "cells/µL", NormalRange = "150,000–350,000 cells/µL", MinValue = 150000, MaxValue = 350000 },
            
         
            new TestParameter { Parameter = "Mean Corpuscular Volume", Abbreviation = "MCV", Unit = "fL", NormalRange = "80–100 fL", MinValue = 80, MaxValue = 100 },
            
     
            new TestParameter { Parameter = "Mean Corpuscular Haemoglobin", Abbreviation = "MCH", Unit = "pg", NormalRange = "27–33 pg", MinValue = 27, MaxValue = 33 },
            
    
            new TestParameter { Parameter = "Mean Corpuscular Haemoglobin Concentration", Abbreviation = "MCHC", Unit = "g/dL", NormalRange = "32–36 g/dL", MinValue = 32, MaxValue = 36 }
        };
    }
}
