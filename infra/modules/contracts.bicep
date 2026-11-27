@export()
@sealed()
type FileShare = {
  @minLength(1)
  registrationName: string
  @minLength(3)
  @maxLength(24)
  accountName: string
  @minLength(3)
  @maxLength(63)
  shareName: string
  @minLength(1)
  accountKeySecretName: string
  @minLength(1)
  mountPath: string
}

@export()
@sealed()
type FileShares = {
  templates: FileShare
  data: FileShare
  pdfArchive: FileShare
  pdfOutbound: FileShare
}

@export()
type ContainerSize = '0.5:1Gi' | '1:2Gi' | '1.5:3Gi' | '2:4Gi' | '2.5:5Gi' | '3:6Gi' | '3.5:7Gi' | '4:8Gi'

@export()
type Tags = {
  @minLength(1)
  application: string
  @minLength(1)
  environment: string
  @minLength(1)
  owner: string
  @minLength(1)
  costCentre: string
  *: string
}
