delete from 
	KeywordAssignments
where
	LocalAccountID = (
		select
			KeywordAssignments.LocalAccountID
		from 
			KeywordAssignments
		full outer join Accounts
		on 
			KeywordAssignments.LocalAccountID = Accounts.LocalAccountID
		where 
			--KeywordAssignments.LocalAccountID is null
			Accounts.LocalAccountID is null)