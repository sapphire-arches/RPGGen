/*
 * METHODOLOGY:
  * The general plan goes like this: First, we are given an initial road. From that, we generate a random number of "child" roads.
  * Each of these child roads is thinner than it's parent.
 * TODO:
  * WILL IMPLEMENT:
   * Nothing ATM. Ideas?
  * MAYBE IMPLEMENT:
   * Road locality checking. Make sure we aren't generating roads too close to another. Ok for now I think.
 */
using System;
using System.Collections.Generic;

namespace RPGGen.TownGeneration.RoadGeneration
{
	public class InTownRoadNetwork
	{
		List<Road> _roads;
		Random _r;
		int _stepNumber;
		
		public int TownRadius {
			get;
			set;
		}
		
		public int StepNumber {
			get { return _stepNumber;}
			private set { _stepNumber = value;}
		}
		
		public InTownRoadNetwork (int TownRadius, Random R)
		{
			this.TownRadius = TownRadius;
			this._r = R;
			this._roads = new List<Road> ();
			this._stepNumber = 0;
		}
		
		public void AddRoad (Road r)
		{
			this._roads.Add (r);
		}
		
		public void GrowOneStep ()
		{
			
		}
	}
}