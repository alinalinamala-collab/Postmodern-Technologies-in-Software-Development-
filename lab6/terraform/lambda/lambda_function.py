import boto3
import urllib.parse
import os

def lambda_handler(event, context):
    # Вказуємо LocalStack як ендпоінт
    endpoint = f"http://{os.environ.get('LOCALSTACK_HOSTNAME', 'localhost')}:4566"
    
    s3 = boto3.client('s3', endpoint_url=endpoint)
    ssm = boto3.client('ssm', endpoint_url=endpoint)
    
    # 5.e Додатковий сервіс: Читаємо конфігурацію з AWS SSM Parameter Store
    param_name = os.environ.get('SSM_PARAM_NAME')
    response = ssm.get_parameter(Name=param_name)
    target_bucket = response['Parameter']['Value']
    
    for record in event['Records']:
        source_bucket = record['s3']['bucket']['name']
        key = urllib.parse.unquote_plus(record['s3']['object']['key'], encoding='utf-8')
        
        # 5.c Копіюємо файл
        copy_source = {'Bucket': source_bucket, 'Key': key}
        s3.copy_object(CopySource=copy_source, Bucket=target_bucket, Key=key)
        
    return {"status": "success", "target_bucket": target_bucket}
