--displays assigned keywords and usernames

declare @KeywordsAndAccountID TABLE (keyword nvarchar(50), accountid uniqueidentifier)

insert into @KeywordsAndAccountID (keyword, accountid)
	(select
		Keywords.keyword, KeywordAssignments.LocalAccountID
	from
		Keywords
	inner join KeywordAssignments
	on Keywords.ID = KeywordAssignments.KeywordID)

select
	keyword, Accounts.FirstName, Accounts.LastName
from
	@KeywordsAndAccountID
inner join Accounts
on accountid = Accounts.LocalAccountID
