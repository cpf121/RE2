
                           
                           
                           
SELECT  *
FROM    ( SELECT TOP ( 1 * 10 )
                    ROW_NUMBER() OVER ( ORDER BY UserInfo.NUIIsRead, Notice.NLevel DESC, Notice.NPlanReleaseTime DESC ) RowNum ,
                    Notice.[Id] ,
                    Notice.[NCategoryId] ,
                    Notice.[NCategoryChildId] ,
                    Notice.[NTitle] ,
                    Notice.[NTitleColor] ,
                    Notice.[NLevel] ,
                    Notice.[NPlanReleaseTime] ,
                    Notice.[NIsForceRead] ,
                    Notice.[NIsForceReply] ,
                    NCategory.NCName CategoryName ,
                    NCategoryChild.NCName CategoryChildName ,
                    ISNULL(UserInfo.NUIIsConcern, 0) AS IsConcern ,
                    ISNULL(UserInfo.NUIIsRead, 0) AS IsRead ,
                    ISNULL(UserInfo.NUIIsReply, 0) AS IsReply ,
                    ISNULL(MessageInfo.ReplyCount, 0) AS ReplyCount 
          FROM      [TCSellCRMPublic].dbo.SCPNotice Notice WITH ( NOLOCK )
                    LEFT JOIN [TCSellCRMPublic].dbo.SCPNoticeUserInfo UserInfo
                    WITH ( NOLOCK ) ON UserInfo.NUINoticeId = Notice.Id
                                       AND UserInfo.NUITcNum = '23182'
                                       AND UserInfo.NUIIsValid = 1
                    LEFT JOIN ( SELECT  NMINoticeId ,
                                        COUNT(ID) AS ReplyCount
                                FROM    [TCSellCRMPublic].dbo.SCPNoticeMessageInfo
                                WHERE   NMIIsValid = 1
                                GROUP BY NMINoticeId
                              ) MessageInfo ON MessageInfo.NMINoticeId = Notice.Id
                    LEFT JOIN [TCSellCRMPublic].dbo.SCPNoticeCategory NCategory
                    WITH ( NOLOCK ) ON Notice.NCategoryId = NCategory.Id
                    LEFT JOIN [TCSellCRMPublic].dbo.SCPNoticeCategory NCategoryChild
                    WITH ( NOLOCK ) ON Notice.NCategoryChildId = NCategoryChild.Id
          WHERE     1 = 1
                    AND Notice.[NCategoryId] = ( SELECT ID
                                                 FROM   [TCSellCRMPublic].dbo.SCPNoticeCategory
                                                 WHERE  NCParentId = 0
                                                        AND NCName = '¹«¸æ'
                                                        AND NCIsValid = 1
                                               )
                    AND Notice.NIsValid = 1
                    AND Notice.NAuditStatus = 1
                    AND ( ( GETDATE() BETWEEN NPlanReleaseTime
                                      AND     Notice.NExpirationTime )
                          OR ( GETDATE() >= NPlanReleaseTime
                               AND Notice.NExpirationTime = '1900-01-01 00:00:00.000'
                             )
                        )
                    AND EXISTS ( SELECT 1
                                 FROM   ( SELECT    [NRRNoticeId]
                                          FROM      SCPNoticeReadRange WITH ( NOLOCK )
                                          WHERE     NRRType = 1
                                                    AND NRRIsValid = 1
                                                    AND NRRValue = '0'
                                          UNION ALL
                                          SELECT    [NRRNoticeId]
                                          FROM      SCPNoticeReadRange
                                          WHERE     NRRType = 2
                                                    AND NRRIsValid = 1
                                                    AND NRRValue IN (
                                                    SELECT  [DURDepartId]
                                                    FROM    [SCPDepartmentUserRelation]
                                                    WHERE   DURUserTCNum = '33261' )
                                          UNION ALL
                                          SELECT    [NRRNoticeId]
                                          FROM      SCPNoticeReadRange
                                          WHERE     NRRType = 3
                                                    AND NRRIsValid = 1
                                                    AND NRRValue = '33261'
                                          UNION ALL
                                          SELECT    [NRRNoticeId]
                                          FROM      SCPNoticeReadRange
                                          WHERE     NRRType = 4
                                                    AND NRRIsValid = 1
                                                    AND NRRValue = '100'
                                        ) AS NoticeTable
                                 WHERE  NoticeTable.[NRRNoticeId] = Notice.Id )
        ) newTable
WHERE   newTable.RowNum > ( ( 1 - 1 ) * 10 )  