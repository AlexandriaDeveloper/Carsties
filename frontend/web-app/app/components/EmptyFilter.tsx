import { useParamsStore } from '@/hooks/useParamsStore';
import { type } from 'os'
import React from 'react'
import Heading from './Heading';
import { Button } from 'flowbite-react';

type Props ={
    title? :string;
    subtitle? :string;
    showReset? :boolean
}

export default function EmptyFilter( {title ='No Match For This Filter',subtitle ='Try Change or resetting the filter',showReset}:Props) {
 const state =  useParamsStore(state => state.reset);
    return (
    <div className='h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg'>
        <Heading title={title} subtitle={subtitle}/>
        <div className='mt-4'>
            {showReset && 
            <Button outline onClick={state} className=''>
                Reset Filters
            </Button>}
        </div>
        
    </div>
  )
}
