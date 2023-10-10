1- Place the ProBuilderRepairDefaultMaterialForHDRP onto a new game object in your scene.
2- Set the ProBuilderDefaultHDRPMaterial to the HDRP compatible default Probuilder material
3- Check ExecuteRepairSequence and wait for it to execute.
3- Once the probuilder materials are fixed, you can delete the ProBuilderRepairDefaultMaterialForHDRP component in the scene
4- Done!

NOTE: This doesn't convert anything else than the default probuilder material. If you have other broken pink materials, you have to convert them manually to HDRP.
Don't forget to use the HDRP Wizard for batch converting all the materials in your project.