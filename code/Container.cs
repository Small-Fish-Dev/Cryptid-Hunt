namespace CryptidHunt;

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

	public Container( string name, float maxWeight = 20f, int? id = null )
	{
		ID = id ?? all.Count;

		Name = name;

		Items = new List<Item>();
		MaxWeight = maxWeight;

		if ( all.ContainsKey( ID ) )
			all.Remove( ID );

		all.Add( ID, this );

		//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Initialize, getInitialUpdate() );
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
			var item = Items[i];
			if ( item?.Resource.ResourceName != resourceName
				|| (ignoreMax && item.Amount >= item.Resource.MaxAmount) ) continue;

			return i;
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
		var res = item.Resource;
		var totalAmount = amount ?? item.Amount;
		var amountToAdd = totalAmount;

		while ( amountToAdd > 0f )
		{
			var index = Find( res.ResourceName, true );

			if ( index != null )
			{
				var found = Items.ElementAtOrDefault( index.Value );
				if ( found != null )
				{
					var fit = Math.Min( amountToAdd, found.Resource.MaxAmount - found.Amount );
					if ( fit > 0f )
					{
						found.Amount += fit;
						amountToAdd -= fit;
						item.Amount -= fit;

						//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Amount, getAmountUpdate( found ) );
						continue;
					}
				}
			}

			var i = Items.Count;
			Items.Insert( i, item );
			item.Index = i;
			item.Container = this;
			item.Amount = amountToAdd;

			//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Insert, getInsertUpdate( item ) );

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
		var item = Items.ElementAtOrDefault( index );
		if ( item == null )
			return false;

		if ( amount != null )
		{
			if ( item.Amount - amount.Value < 0f )
				return false;

			Weight -= amount.Value * item.Resource.Weight;
			item.Amount -= amount.Value;

			if ( item.Amount == 0 )
			{
				Items.RemoveAt( index );
				//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Remove, getRemoveUpdate( index ) );

				return true;
			}

			//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Amount, getAmountUpdate( item ) );

			return true;
		}

		item.Container = null;
		Weight -= item.Amount * item.Resource.Weight;
		Items.RemoveAt( index );
		//Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Remove, getRemoveUpdate( index ) );

		return true;
	}

	public override string ToString()
	{
		var result = $"{Name}\n";
		for ( int i = 0; i < Items.Count; i++ )
			result += $"{Items?[i]?.Resource.Title} " +
				$"({Items?[i]?.Amount}/{Items?[i]?.Resource.MaxAmount})" +
				$"{(i < Items.Count - 1 ? "," : "")}";

		return $"{result}\nWeight: {Weight}/{MaxWeight} KG";
	}
}
