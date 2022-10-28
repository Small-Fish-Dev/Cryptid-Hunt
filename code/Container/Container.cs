namespace SpookyJam2022;

public partial class Container
{
	private static Dictionary<int, Container> all = new();
	public static IReadOnlyDictionary<int, Container> All => all;

	public string Name { get; private set; }
	public int ID { get; private set; }

	public float MaxWeight { get; private set; }

	private float weight;
	public float Weight 
	{ 
		get => weight;
		set => weight = Math.Max( value, 0 );
	}

	public List<Item> Items { get; private set; }

	public Item this[int index]
	{
		get
		{
			return Items.ElementAtOrDefault( index );
		}
	}

	public Container( string name, float maxWeight = 20f, int? id = null, Client target = null )
	{
		ID = id ?? all.Count;

		Name = name;
		
		Items = new List<Item>();
		MaxWeight = maxWeight;

		if ( all.ContainsKey( ID ) )
			all.Remove( ID );

		all.Add( ID, this );

		if ( target != null )
			UpdateTargets.Add( target );

		if ( Host.IsServer )
			Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Initialize, getInitialUpdate() );
	}

	/// <summary>
	/// Looks for an item of specific resource name.
	/// </summary>
	/// <param name="resourceName"></param>
	/// <param name="ignoreMax"></param>
	/// <returns></returns>
	public int? Find( string resourceName, bool ignoreMax = false )
	{
		for ( int i = 0; i < Items.Count; i++ )
		{
			if ( Items[i]?.Resource.ResourceName == resourceName 
				&& (ignoreMax && Items[i].Amount < Items[i].Resource.MaxAmount ) ) return i;
		}

		return null;
	}

	/// <summary>
	/// Inserts an item to the container.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="amount"></param>
	public void Insert( Item item, float? amount = null )
	{
		Host.AssertServer();

		var res = item.Resource;
		var totalAmount = amount ?? item.Amount;
		var amountToAdd = totalAmount;

		while ( amountToAdd > 0f )
		{
			var index = Find( res.ResourceName, true );

			if ( index != null )
			{
				var existing = Items.ElementAtOrDefault( index ?? Items.Count );
				if ( existing != null )
				{
					var fit = Math.Min( item.Amount, existing.Resource.MaxAmount - existing.Amount );
					if ( fit > 0f )
					{
						existing.Amount += fit;
						amountToAdd -= fit;
						item.Amount -= fit;

						Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Amount, getAmountUpdate( existing ) );
						continue;
					}
				}
			}

			var i = index ?? Items.Count;
			Items.Insert( i, item );
			item.Index = i;
			item.Container = this;

			Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Insert, getInsertUpdate( item ) );

			break;
		}

		Weight += res.Weight * totalAmount;
	}

	/// <summary>
	/// Removes a item from some position.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="amount"></param>
	/// <returns>A boolean telling you if the item was successfully removed.</returns>
	public bool Remove( int index, float? amount = null )
	{
		Host.AssertServer();

		var item = Items.ElementAtOrDefault( index );
		if ( item == null ) 
			return false;

		if ( amount != null )
		{
			if ( item.Amount - amount.Value < 0f )
				return false;

			Weight -= amount.Value * item.Resource.Weight;
			item.Amount -= amount.Value;
			
			Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Amount, getAmountUpdate( item ) );

			return true;
		}

		item.Container = null;
		Weight -= item.Amount * item.Resource.Weight;
		Items.RemoveAt( index );
		Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Remove, getRemoveUpdate( index ) );
		
		return true;
	}

	public override string ToString()
	{
		var result = $"[{(Host.IsServer ? "SV" : "CL")}] {Name}\n";
		for ( int i = 0; i < Items.Count; i++ )
			result += $"{Items?[i]?.Resource.Title} " +
				$"({Items?[i]?.Amount}/{Items?[i]?.Resource.MaxAmount})" +
				$"{(i < Items.Count - 1 ? "," : "")}";

		return $"{result}\nWeight: {Weight}/{MaxWeight} KG";
	}
}
