using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace TuringMachine {
	class OTuringMachine {
    const int startPos = 1;

		public struct SRule{
			public int st1, st2;
			public char ch1, ch2;
			public char dirn;
		};
		public OTuringMachine()
		{
			m_pos = startPos; m_state = 0;
			m_regs = new List<char>();
			m_rules = new List<SRule>();
			m_empty_char = '_';
		}

		public int addRule(string rule)
		{

		  if (rule[0] =='/' && rule [1] == '/')
		    return 0;
			Regex ex = new Regex("(\\S)q(\\d+)->(\\S)q(\\*|\\d+)([NLR]?)");
			Match m = ex.Match(rule);
			SRule r;

			if( !m.Success ) { return -1; }
			
			r.ch1 = char.Parse(m.Groups[1].Value);
			r.st1 = int.Parse(m.Groups[2].Value);
			r.ch2 = char.Parse(m.Groups[3].Value);
			
			if( !int.TryParse(m.Groups[4].Value, out r.st2) ) { r.st2 = -1; }
			
			if( m.Groups[5].Value.Length != 1 ) { r.dirn = 'N'; }
			else { r.dirn = char.Parse(m.Groups[5].Value); }
			m_rules.Add(r);
			return 1;
		}
/*
		public int addRules(string[] rules)
		{
			int added = 0;
			for( int i = 0; i < rules.Length; i++ )
			{ int rc = addRule(rules[i]);
			                     
				if( rc > 0 ) { 
				   added++; 
				}
				else if (rc < 0) {
				   Console.Error.WriteLine("Wrong line {0}: '{1}'", i, rules [i]);
				}
			}
			return added;
		}
*/
		public void removeRules() { m_rules.Clear(); }
		public string rule(int ix)
		{
			string res;
			if( ix >= m_rules.Count ) { Console.Error.WriteLine("Error"); }
			if( ix >= m_rules.Count ) { Console.Error.WriteLine("Error"); }
			if( m_rules[ix].st2 == -1 ) res = string.Format("*{0}", m_rules[ix].dirn);
			else res = string.Format("{0}{1}", m_rules[ix].st2, m_rules[ix].dirn);

			return string.Format("{0}q{1}->{2}q",
				m_rules[ix].ch1, m_rules[ix].st1,
				m_rules[ix].ch2) + res;
		}
		
		public int currentState() { return m_state; }

		public void setRegisters(char[] symbols) { m_regs = symbols.ToList(); m_pos = startPos; }
		public void setRegisters(List<char> symbols) { m_regs = symbols; m_pos = startPos; }
		public List<char> registersList() { return m_regs; }
		public char[] registersArray() { return m_regs.ToArray(); }
		
		public char blankSymbol() { return m_empty_char; }
		public void setBlankSymbol(char symbol) { m_empty_char = symbol; }

		public int cursorPosition() { return m_pos; }
		public bool isFinished() { return m_state == -1; }
		// result - number of rule matched
		public int execStep()
		{
			for( int i = 0; i < m_rules.Count; i++ )
			{
				if( m_state == -1 ) { return -1; }
				if( m_rules[i].ch1 == m_regs[m_pos]
				&&  m_rules[i].st1 == m_state )
				{
					m_regs[m_pos] = m_rules[i].ch2;
					m_state = m_rules[i].st2;
					
					if( m_rules[i].dirn == 'R' ) { m_pos++; }
					else if( m_rules[i].dirn == 'L' ) { m_pos--; }
					
					if( m_pos == m_regs.Count ) { m_regs.Add(m_empty_char); }
					else if( m_pos < 0 ) { m_regs.Insert(0, m_empty_char); m_pos = 0; }
					
					return i;
				}
			}
			return -1;
		}
		char m_empty_char;
		List<char> m_regs; // list of register values
		List<SRule> m_rules; // list of rules
		int m_state, m_pos;
	}
	
	class Program {
		static void help()
		{
			Console.Error.Write(
@"The program implements some variant of Turing machime. To use the machine you
should set staring configuration of registers and list of rules to define an
algorithm. Also you can set custom blank symbol (which is value of empty
registers) and time interval between iterations.

String of registers is endless in both sides from head. The head can move
forward and backward by registers and change their values. Rules show the
machine how does the values are changed and head moving if current head state
has some value described in rules.

At the beginning head stands at the leftmost nonblank register. When performing
step the machine scans list of rules, finding one which matches current symbol
(value of register under the head) and current head state. First foundmatch is
executed. Process of calculations continues untill current state number become
-1 (symbol '*').

tm1.exe [-v] [-r registers] [-i interval] [-b blank]  [-f inputFile] [-o outputFile]

Command line options:
	-v            - allow the program to write explanations and questions.
	                Use this at least for the first time.
	-r registers  - set starting configuration of registers as string of chars
	                (each char is separate register value)
	-i interval   - set time interval between steps in seconds (floating point
	                numbers allowed. Something like -i 1,5)
	-b blank      - set blank symbol to be placed in empty registers
	-h | -?            - see this manual.
	-f inputFile or stdin
	-o outputFile or stdout

The rule format is:
	<symbol1>q<state1_number>-><symbol2><state2_number>{N|R|L}
	Examples: *q1->%q3R , |q23->*q* , Tq1->#q3N ...
Where:
	<symbol1>  - any not blank symbol.
	<symbol2>  - not blank symbol which replaces <symbol1>.
	R          - indicates head movement to the next right register after setting
	             of current register value.
	L          - indicates left movement.
	N          - indicates that the head does`t move.");
		}

		static int Main( string[] args )
		{
			bool v_flag = false;
			string inFlNm = "";
			string outFlNm = "";
			bool d_flag = false;
			double interval = 0.0;
			char[] regs = null;
			char empty = '_';
			
			for( int i = 0; i < args.Length; i++ )
			{
				switch( args[i] ) {
					case "-h":
						help();
						return 1;
					case "-?":
						help();
						return 1;
					case "-v":
						v_flag = true; break;
					case "-d":
						d_flag = true; break;
					case "-r":
						if( ++i == args.Length )
							{ Console.Error.WriteLine("ERROR: unexpected command line end"); return 1; }
						regs = args[i].ToCharArray();
						break;
					case "-f":
						if( ++i == args.Length )
							{ Console.Error.WriteLine("ERROR: unexpected command line end"); return 1; }
						inFlNm = args[i];
						break;
					case "-o":
						if( ++i == args.Length )
							{ Console.Error.WriteLine("ERROR: unexpected command line end"); return 1; }
						outFlNm = args[i];
						break;
					case "-b":
						if( ++i == args.Length )
							{ Console.Error.WriteLine("ERROR: unexpected command line end"); return 1; }
						empty = args[i][0];
						break;
					case "-i":
						if( ++i == args.Length )
							{ Console.Error.WriteLine("ERROR: unexpected command line end"); return 1; }
						interval = double.Parse(args[i]);
						break;
					
					default:
						Console.Error.WriteLine("ERROR: undefined option");
						help();  return 1;
				}
			}
			if (d_flag)
						Console.Error.WriteLine("input /output file names: '{0}'/'{1}'", inFlNm, outFlNm);
			
			OTuringMachine machine = new OTuringMachine();
			machine.setBlankSymbol(empty);

			StreamReader  rdr;
			StreamWriter wrt;
			Encoding enc1 = Encoding.GetEncoding(1251);

			  Stream    so ;// = Console.OpenStandardOutput();


			if (string.IsNullOrEmpty(inFlNm))
			    rdr = new StreamReader(Console.OpenStandardInput());
			else 
			    rdr = new StreamReader(inFlNm, false);

			if (string.IsNullOrEmpty(outFlNm))  {
			    so = Console.OpenStandardOutput();
			    wrt = new StreamWriter(so);

      }
			else  {
			    wrt = new StreamWriter(outFlNm);
			    so = wrt.BaseStream;

			}

			byte [] b= new  byte[1];
	 /*
			wrt.WriteLine("ddddd");
			b = enc1.GetBytes("dddddd");
      so.Write(b, 0, (int)(b.Length));
   */
 			
			if( regs==null )
			{
				if( v_flag ) { Console.Error.WriteLine("Input starting configuration of registers (string of chars):\n"); }
				regs = rdr.ReadLine().ToArray();
			}
			machine.setRegisters(regs);

			if( v_flag ) { Console.Error.Write("Input list of rules:\n"); }
			int lineno = 0;
			while( !rdr.EndOfStream )
			{
				string str = rdr.ReadLine();
				lineno++;
				if( machine.addRule(str) < 0 )
				{
//					if( v_flag ) { Console.WriteLine("\tERROR: Rule not added: {0}", str); }
				   Console.Error.WriteLine("Wrong line {0}: '{1}'", lineno, str);
				}
			}
			if( v_flag ) { Console.Error.Write("Executing...\n"); }

			for( int s = 0;; s++ )
			{
				if( machine.isFinished() ) { 
				  if( v_flag ) Console.Error.Write("Process ended:  "); 
    			for( int i=0; i < machine.registersArray().Length; i++ )
    				{ 
//			wrt.WriteLine("ddddd");//
		//	byte [] b= new  byte[1];
		//	b = enc1.GetBytes("dddddd");
    //  so.Write(b, 0, (int)(b.Length));

            string str = (machine.registersArray()[i]).ToString();
            b = enc1.GetBytes(str);
            so.Write(b, 0, (int)(b.Length));
      
    	//			wrt.Write("{0}", machine.registersArray()[i]); 
    				//Console.Write("{0}", machine.registersArray()[i]); 
    				
    				}
				  return 0; 
				}


				if( v_flag ) 
					Console.Error.Write(" STEP[{0}]\n", s+1);

				if( v_flag ) {
					Console.Error.Write("Head position: ");
					for( int i = 0; i < machine.cursorPosition(); i++ ) Console.Error.Write(" ");
					Console.Error.Write(".\n");
				  //Console.WriteLine(machine.cursorPosition()); 
				}
				
				if( v_flag ) { 
				  Console.Error.Write("Registers    : "); 
  				for( int i=0; i < machine.registersArray().Length; i++ )
  					{ Console.Error.Write("{0}", machine.registersArray()[i]); }
				}

				if( v_flag ) { 
				  Console.Error.Write("\nCurrent state: "); 
					Console.Error.WriteLine("{0}", machine.currentState());
				}


				int step_res = machine.execStep();

 			  if( step_res < 0 )
 				{
 					Console.Error.Write("\nERROR: No rule match. Stop. Cursor position: {0}\nCurrent symbol: {1}\n\n",
 						machine.cursorPosition(),
 						machine.registersArray()[machine.cursorPosition()]
 				  );
 					return 1;
 				}
 				else { 
 				  if( v_flag )
 				    Console.Error.Write("Rule number {0} matched ({1})\n", step_res+1, machine.rule(step_res)); 
 				}
				
				
				if (v_flag)
			  	Console.Error.WriteLine();
				
				Thread.Sleep((int)(interval * 1000));
			}
		}
	}
}
