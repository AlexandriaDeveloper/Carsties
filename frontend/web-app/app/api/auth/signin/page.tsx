
import EmptyFilter from '@/app/components/EmptyFilter'
import React from 'react'

export default function Page({searchParams} :{searchParams :{callbackUrl : string}}) {
  return (
        <EmptyFilter 
        title='you need to login'
        subtitle='Please click below to sign in'
        showLogin
        callbackUrl={searchParams.callbackUrl} />
  )
}
