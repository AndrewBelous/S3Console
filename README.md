# S3Console
Simple command-line S3 browser.

Available actions are:
        list-buckets
        list
        delete
        set-bucket
        gen-download-url
        page-size
Get action specific help with "[action]:"

list-buckets:
Usage: "list-buckets"

list:
Usage: "list [bucket] [key prefix]"
        Ex: "list mybucket.bucket.com xml/967871 "

delete:
Usage: "delete [bucket] [key]"
        Ex: "delete mybucket.bucket.com xml/967871/blah.xml"

set-bucket:
Usage: "set-bucket [bucket]"

gen-download-url:
Usage: "gen-download-url [bucket] [key]"
        Ex: "gen-download-url mybucket.bucket.com xml/967871/blah.xml "