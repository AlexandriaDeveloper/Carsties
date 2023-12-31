'use client'
import { useParamsStore } from '@/hooks/useParamsStore'
import { stat } from 'fs';
import { usePathname, useRouter } from 'next/navigation';
import React, { use, useState } from 'react'
import {FaSearch} from 'react-icons/fa'
export default function Search() {
    const router = useRouter();
    const pathname = usePathname();
    const setParams = useParamsStore(state => state.setParams);
    const setSearchValue = useParamsStore(state => state.setSearchValue);
    const searchValue = useParamsStore(state => state.searchValue )

    function onChange(event :any){
        setSearchValue(event.target.value);
    }
    function search(){
        if(pathname !== '/'){
            router.push('/')
        }
        setParams({searchTerm:searchValue})
    }
  return (
    <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
        <input type='text' placeholder='Search For Car By Model or Color ....'
        value={searchValue}
        onChange={onChange}
        onKeyDown={(e :any) =>{
            if(e.key==='Enter')
            search(); 
        }}
        className='
        input-custom
        text-sm 
        text-gray-600'
        ></input>
        <button onClick={search}>
            <FaSearch size={34} className='bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2' />
        </button>
    </div>
  )
}
