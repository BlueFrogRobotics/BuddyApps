/**
/**
GENERALITIES
/**
**/

/*
    No space within folder or file name. Never. Please.
    Files must be in PascalCase
	
    Use friendly names for variables, methods and classes
    Absolutely avoid a, k, j...
    You can use C# struct but don't forget that method call with struct it is by value and not reference
	
    Full english comments (but we prefer readable code than messy useless comments)
	
    Some Unity functions are very unrecommanded like Find. Use any other way
    Some Unity objects (like Texture2D) are not handle by the garbage collector, you have to manage it by yourself
	Make a class inherit from Monobehaviour only if needed (Update system, enable, disable, access to resources...)
	
	Same line for if / for brackets
	New line for any other thing (class, interface, method) brackets
	
	Global within-hierarchy file :
	- public Const attributes
	- private Const attributes
	- private Static attributes
	- Serialized attributes
	- Protected attributes
	- Private attributes
	- Getter/Setter
	- [Instance getter for singleton]
	- [Awake for Mono]
	- [Start for Mono]
	- [Update for Mono]
	- [OnEnable for Mono]
	- [OnDisable for Mono]
    - [OnDestroy for Mono]
	- [Constructor for logic class]
	- Inherited (override) public methods
	- Public methods
    - Inherited (override) internal methods
    - Internal methods
	- Inherited (override) protected methods
	- Protected methods
	- Private methods
 */

using UnityEngine;

/* Use namespaces for naming and hierarchy */
namespace BuddySample.Doc
{
    /**
	/**
	CLASSES, STRUCTS and INTERFACES
	/**
	**/

    /* Struct naming like class : explicit */
    /* ALWAYS keep basic type member in struct */
    /* DO NOT use struct if the total byte count if higher than 16 bytes */
    /* DO NOT use object as member in struct */
    /* DO NOT create struct constructor, use instead : 
        Point lPoint = new Point() { 
            X = 5,
            Y = 3
        };
    */
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    /* Interfaces start by "I" + Pascalcase*/
    public interface IInterface
    { /* New line bracket */
    }

    /* Abstract classes start by "A" + Pascalcase */
    public abstract class AClass
    { /* New line bracket */
    }

    /* Enum are in Pascalcase */
    /* It's preferable to inherit from int for better casting */
    /* Values of the enum must be in UPPER_CASE */
    public enum MyType : int
    { /* New line bracket */
        FIRST,
        SECOND,
        THE_THIRD
    }

    /* Class name must be explicit and in PascalCase */
    public class SubClass : AClass /* For subclass, the name must be in relationship to the mother class */
    {
        /**
		/**
		MEMBERS
		/**
		**/

        /* Never public member (Except const if needed) */
        /* Use SerializeField attribute to make them visible from Editor (only if you need it) */
        /* Class member for Unity must be in camelCase (before it was like classic class member) */
        [SerializeField]
        private int member;

        /* ONE blank line between serialized member */

        [SerializeField]
        private string name;

        /* Constants must be in UPPER_CASE */
        private const int MAX_THRESHOLD = 5;

        /* Static member starts by a "s" followed by a PascalCase */
        private static int sCount;

        /* Class member must start by "m" followed by a PascalCase */
        /* Member are set in constructor / Awake / Start method, NOT in the declaration */
        private int mMember;

        /**
		/**
		GETTERS / SETTERS
		/**
		**/

        /* For classic value get set (no need to private member) */
        public int GetterSetter { get; set; } /* You can let the get/setter on the same line */


        /* If you need to make a specific operation after setting, use the setter method */
        public int GetterSetterMember
        {
            get
            {
                return mMember;
            }

            set
            {
                mMember = value;
                /* Specific operations with the new value */
            }
        }

        /* If a member must be in readonly for outer classes, specify a getter only */
        public int Getter { get { return mMember; } }

        /**
		/**
		METHODS
		/**
		**/

        /* Always precise method visibility. Methods must in PascalCase */
        /* Do not exceed more than 4 arguments by method */
        /* Avoid exceed more than 40 lines by method */

        /* Reminder : 
		/* basic types + string + struct ==> copy */
        /* Object ==> reference of the object */

        /* Public methods are above private methods and after attribute(s), constructor(s) and getter(s)/setter(s) */
        public int MyFirstMethod()
        {
            return 42;
        }

        /* ONE blank line between methods */

        /* Input arguments must start by "i" followed by PascalCase */
        public void MyBasicMethodWithInput(int iMyInput)
        {
        }

        /* You can add "io" prefix + PascalCase for callback inputs, "i" is still mandatory if not "io" */
        /* Use callback for Coroutine purpose mainly */
        public void MyCallBack(out int ioMyInputCallback)
        {
            ioMyInputCallback = 5;
        }

        /* Use C# commenting system */

        /// <summary>
        /// Concat the input char with the 'Poney' prefix
        /// </summary>
        /// <param name="iCharToConcat">The input char to concat</param>
        /// <returns>The string "Poney" followed by a space then the input char</returns>
        public string MyCommentedMethod(char iCharToConcat)
        {
            return "Poney " + iCharToConcat; /* No obligation to declare explicit output variable for short methods */
        }

        private int Method(int iMyFirstInput, int iMySecondInput) /* Space after coma */
        {
            /* Declarations first */
            /* Local variable starts by "l" followed by a PascalCase */
            /* Variable type must be EXPLICIT (no var declaration) */
            /* Exception for i, j, k in for loops */
            int lLocalVariableInt = 4; /* One instruction by line */
            float lLocalVariableFloat = 4F; /* Precise F for float */
            double lLocalVariableDouble = 4D; /* Precise D for double */
            int lSum = iMyFirstInput + iMySecondInput; /* Avoid redundant operation. Be CPU friendly. */
            int oResultOfMethod = lSum * 2;

            /* Process secondly */
            if (oResultOfMethod > 20 && oResultOfMethod < 40) { /* Space between any binary operator */ /* Same line bracket */
                int lLocalVariableBeforeLoop = 5; /* Declare variables BEFORE any loop */
                int lValueFloat = (int)lLocalVariableFloat;
                int lValueDouble = (int)lLocalVariableDouble;
                for (int i = 0; i < 42; ++i) /* Prefer post incrementation for loop */
                    oResultOfMethod += lLocalVariableBeforeLoop + lLocalVariableInt + lValueFloat + lValueDouble;
            }

            return oResultOfMethod; /* Output must start by "o" and followed by PascalCase */
        }
    }
}

/**
/**
OPTIMIZATIONS
/**
**/

/*
- Use System.String.Empty, not ""
- For quick fill and clear operation, use List, not Dictionnary
- Avoid LINQ
     
*/