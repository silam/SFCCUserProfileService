# Example of User Profile json

{
    "person_key": 12332112,
    "rwsc_employee": false,
    "first_name": "Chas",
    "last_name": "Someone",
    "profile": {
        "addresses": [
            {
                "type": "home",
                "delivery": "1500 E Main Ste 201",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1010"
            },
            {
                "type": "shipping",
                "delivery": "1313 Mockingbird Lane",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1030"
            },
            {
                "type": "billing",
                "delivery": "1500 E Main Ste 201",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1010"
            }
        ],
        "emails": [
            {
                "personal": "somebody@something.com",
                "validated": true
            },
            {
                "work": "IworkHere@myjob.com",
                "validated": false
            },
            {
                "other": "hahaha@gmail.com",
                "validated": false
            }
        ],
        "phones": [
            {
                "type": "mobile",
                "number": "8609221234",
                "sms_capable": true,
                "validated": true
            },
            {
                "type": "home",
                "number": "8609224321",
                "sms_capable": false,
                "validated": false
            }
        ],
        "occupation": "an occupation code or free text",
        "preferencess": {
            "communication": "text",
            "receipt": "paper"
        },
        "industrial_account": {
            "account_name": "Test account for Ecommerce",
            "program_name": "$100 subsidy - ST",
            "program_id": "P-282189",
            "last_transaction_date": "2023-06-13T14:00:00Z",
            "voucher_type": "digital"
        },
        "home_store": "RWSS309",
        "ufx_information": {
            "last_scan_date": "2023-05-04T12:34:00Z",
            "last_scan_url": "http://somesite.com/13125323"
        },
        "related_accounts": [
            12321123,
            12343421,
            53322213
        ],
        "discounts": [],
        "profile_notes": ""
    },
    "id": "25eafa50-dd46-4d8e-88ac-f48a2887b283",
  
}
