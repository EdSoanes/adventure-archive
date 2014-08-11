using System;

namespace BlackDragon.Core
{
	public enum ContentTypes
	{
		Content,
		Favourites,
		Library,
		Map,
		Notes,
		Search
	}

	public enum FileCacheTypes
	{
		Unknown,
		Jpg,
		Png,
		Txt,
		Json,
		Xml
	}

	public enum StatusCode
	{
		Ok,
		UnknownErrorOccured,
		HttpFileNotFound,
		HttpInvalidUrl,
		FileAccessError,
		JsonDeserializeFailed,
		UserDataWorldNotSelected,
		UserDataWorldAlreadyExists
	}
}

