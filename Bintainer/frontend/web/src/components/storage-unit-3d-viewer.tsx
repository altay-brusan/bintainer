"use client";

import { useRef, useState, useCallback } from "react";
import { Canvas, useThree, type ThreeEvent } from "@react-three/fiber";
import { OrbitControls, RoundedBox, Text, Environment } from "@react-three/drei";
import * as THREE from "three";

interface BinData {
  id: string;
  row: number;
  col: number;
  hasComponents: boolean;
  label?: string;
}

interface StorageUnit3DViewerProps {
  rows: number;
  columns: number;
  bins: BinData[];
  selectedBin: { row: number; col: number } | null;
  onBinSelect: (row: number, col: number) => void;
  deletedBins?: Set<string>;
  removedBins?: Set<string>;
  onBinDoubleClick?: (row: number, col: number) => void;
  onBinRestore?: (row: number, col: number) => void;
}

// Bin dimensions (relative units)
const BIN_WIDTH = 1.0;
const BIN_HEIGHT = 0.7;
const BIN_DEPTH = 1.2;
const GAP = 0.08;

// Colors
const COLORS = {
  empty: "#94A3B8",
  emptyHover: "#B0BEC5",
  occupied: "#60A5FA",
  occupiedHover: "#93C5FD",
  selected: "#4F6EF7",
  selectedGlow: "#818CF8",
  shelf: "#334155",
  label: "#ffffff",
};

function Bin({
  position,
  row,
  col,
  hasComponents,
  isSelected,
  isDeleted,
  label,
  onSelect,
  onDoubleClick,
}: {
  position: [number, number, number];
  row: number;
  col: number;
  hasComponents: boolean;
  isSelected: boolean;
  isDeleted: boolean;
  label: string;
  onSelect: (row: number, col: number) => void;
  onDoubleClick: (row: number, col: number) => void;
}) {
  const meshRef = useRef<THREE.Mesh>(null);
  const [hovered, setHovered] = useState(false);

  const color = isDeleted
    ? "#EF4444"
    : isSelected
      ? COLORS.selected
      : hovered
        ? hasComponents
          ? COLORS.occupiedHover
          : COLORS.emptyHover
        : hasComponents
          ? COLORS.occupied
          : COLORS.empty;

  const handleClick = useCallback(
    (e: ThreeEvent<MouseEvent>) => {
      e.stopPropagation();
      onSelect(row, col);
    },
    [onSelect, row, col]
  );

  const handleDoubleClick = useCallback(
    (e: ThreeEvent<MouseEvent>) => {
      e.stopPropagation();
      onDoubleClick(row, col);
    },
    [onDoubleClick, row, col]
  );

  return (
    <group position={position}>
      {/* Main bin body */}
      <RoundedBox
        ref={meshRef}
        args={[BIN_WIDTH, BIN_HEIGHT, BIN_DEPTH]}
        radius={0.03}
        smoothness={4}
        onClick={handleClick}
        onDoubleClick={handleDoubleClick}
        onPointerOver={(e) => {
          e.stopPropagation();
          setHovered(true);
          document.body.style.cursor = "pointer";
        }}
        onPointerOut={() => {
          setHovered(false);
          document.body.style.cursor = "auto";
        }}
      >
        <meshStandardMaterial
          color={color}
          transparent
          opacity={isDeleted ? 0.2 : isSelected ? 1 : 0.85}
          roughness={0.3}
          metalness={0.1}
        />
      </RoundedBox>

      {/* Front face label */}
      <Text
        position={[0, 0, BIN_DEPTH / 2 + 0.001]}
        fontSize={0.15}
        color={isDeleted ? "#EF4444" : COLORS.label}
        anchorX="center"
        anchorY="middle"
        font={undefined}
      >
        {label}
      </Text>

      {/* Red X overlay for deleted bins */}
      {isDeleted && (
        <Text
          position={[0, 0, BIN_DEPTH / 2 + 0.01]}
          fontSize={0.4}
          color="#EF4444"
          anchorX="center"
          anchorY="middle"
          font={undefined}
        >
          ✕
        </Text>
      )}

      {/* Selection glow */}
      {isSelected && !isDeleted && (
        <RoundedBox
          args={[BIN_WIDTH + 0.06, BIN_HEIGHT + 0.06, BIN_DEPTH + 0.06]}
          radius={0.04}
          smoothness={4}
        >
          <meshStandardMaterial
            color={COLORS.selectedGlow}
            transparent
            opacity={0.2}
            roughness={1}
          />
        </RoundedBox>
      )}

      {/* Drawer handle detail */}
      {!isDeleted && (
        <mesh position={[0, -0.05, BIN_DEPTH / 2 + 0.02]}>
          <boxGeometry args={[0.3, 0.04, 0.02]} />
          <meshStandardMaterial
            color={isSelected ? "#c7d2fe" : "#cbd5e1"}
            roughness={0.2}
            metalness={0.4}
          />
        </mesh>
      )}
    </group>
  );
}

function GhostBin({
  position,
  row,
  col,
  onRestore,
}: {
  position: [number, number, number];
  row: number;
  col: number;
  onRestore: (row: number, col: number) => void;
}) {
  const [hovered, setHovered] = useState(false);

  return (
    <group position={position}>
      <RoundedBox
        args={[BIN_WIDTH, BIN_HEIGHT, BIN_DEPTH]}
        radius={0.03}
        smoothness={4}
        onDoubleClick={(e) => { e.stopPropagation(); onRestore(row, col); }}
        onPointerOver={(e) => { e.stopPropagation(); setHovered(true); document.body.style.cursor = "pointer"; }}
        onPointerOut={() => { setHovered(false); document.body.style.cursor = "auto"; }}
      >
        <meshStandardMaterial
          color={hovered ? "#94A3B8" : "#475569"}
          transparent
          opacity={hovered ? 0.3 : 0.08}
          roughness={0.8}
          wireframe={!hovered}
        />
      </RoundedBox>
    </group>
  );
}

function ShelfUnit({
  rows,
  columns,
  bins,
  selectedBin,
  onBinSelect,
  deletedBins,
  removedBins,
  onBinDoubleClick,
  onBinRestore,
}: StorageUnit3DViewerProps) {
  const totalWidth = columns * (BIN_WIDTH + GAP) - GAP;
  const totalHeight = rows * (BIN_HEIGHT + GAP) - GAP;
  const offsetX = -totalWidth / 2 + BIN_WIDTH / 2;
  const offsetY = -totalHeight / 2 + BIN_HEIGHT / 2;

  const binMap = new Map<string, BinData>();
  for (const bin of bins) {
    binMap.set(`${bin.row}-${bin.col}`, bin);
  }

  return (
    <group>
      {/* Back panel */}
      <mesh position={[0, 0, -BIN_DEPTH / 2 - 0.05]}>
        <boxGeometry args={[totalWidth + 0.3, totalHeight + 0.3, 0.05]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Side panels */}
      <mesh position={[-(totalWidth + 0.3) / 2, 0, 0]}>
        <boxGeometry args={[0.05, totalHeight + 0.3, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>
      <mesh position={[(totalWidth + 0.3) / 2, 0, 0]}>
        <boxGeometry args={[0.05, totalHeight + 0.3, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Bottom shelf */}
      <mesh position={[0, -(totalHeight + 0.3) / 2, 0]}>
        <boxGeometry args={[totalWidth + 0.3, 0.05, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Shelf dividers (horizontal) */}
      {Array.from({ length: rows - 1 }).map((_, r) => (
        <mesh
          key={`shelf-h-${r}`}
          position={[0, offsetY + (r + 1) * (BIN_HEIGHT + GAP) - GAP / 2 - BIN_HEIGHT / 2, 0]}
        >
          <boxGeometry args={[totalWidth + 0.2, 0.03, BIN_DEPTH + 0.1]} />
          <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
        </mesh>
      ))}

      {/* Bins */}
      {Array.from({ length: rows }).map((_, r) =>
        Array.from({ length: columns }).map((_, c) => {
          const key = `${r}-${c}`;
          const x = offsetX + c * (BIN_WIDTH + GAP);
          const y = offsetY + (rows - 1 - r) * (BIN_HEIGHT + GAP);
          if (removedBins?.has(key)) {
            return (
              <GhostBin
                key={key}
                position={[x, y, 0]}
                row={r}
                col={c}
                onRestore={onBinRestore ?? (() => {})}
              />
            );
          }
          const binData = binMap.get(key);
          const label = `${String(r + 1).padStart(2, "0")}-${String(c + 1).padStart(2, "0")}`;

          return (
            <Bin
              key={key}
              position={[x, y, 0]}
              row={r}
              col={c}
              hasComponents={binData?.hasComponents ?? false}
              isSelected={
                selectedBin?.row === r && selectedBin?.col === c
              }
              isDeleted={deletedBins?.has(key) ?? false}
              label={label}
              onSelect={onBinSelect}
              onDoubleClick={onBinDoubleClick ?? (() => {})}
            />
          );
        })
      )}
    </group>
  );
}

function CameraSetup({ rows, columns }: { rows: number; columns: number }) {
  const { camera, size } = useThree();
  const totalWidth = columns * (BIN_WIDTH + GAP) - GAP + 0.3;
  const totalHeight = rows * (BIN_HEIGHT + GAP) - GAP + 0.3;

  if (camera instanceof THREE.PerspectiveCamera) {
    const fov = camera.fov * (Math.PI / 180);
    const aspect = size.width / size.height;
    // Calculate distance needed to fit the model
    const distH = totalHeight / (2 * Math.tan(fov / 2));
    const distW = totalWidth / (2 * Math.tan(fov / 2) * aspect);
    const dist = Math.max(distH, distW) + BIN_DEPTH;
    camera.position.set(0, 0, dist);
    camera.lookAt(0, 0, 0);
  }

  return null;
}

export function StorageUnit3DViewer({
  rows,
  columns,
  bins,
  selectedBin,
  onBinSelect,
  deletedBins,
  removedBins,
  onBinDoubleClick,
  onBinRestore,
}: StorageUnit3DViewerProps) {
  return (
    <div className="h-[500px] w-full rounded-xl border bg-gradient-to-b from-slate-900 to-slate-800 overflow-hidden">
      <Canvas
        camera={{ fov: 45, near: 0.1, far: 100 }}
        shadows
      >
        <CameraSetup rows={rows} columns={columns} />
        <ambientLight intensity={0.5} />
        <directionalLight position={[5, 8, 5]} intensity={0.8} />
        <directionalLight position={[-3, 4, -2]} intensity={0.3} />
        <pointLight position={[0, 0, 5]} intensity={0.4} />

        <ShelfUnit
          rows={rows}
          columns={columns}
          bins={bins}
          selectedBin={selectedBin}
          onBinSelect={onBinSelect}
          deletedBins={deletedBins}
          removedBins={removedBins}
          onBinDoubleClick={onBinDoubleClick}
          onBinRestore={onBinRestore}
        />

        <OrbitControls
          enablePan
          enableZoom
          enableRotate
          minDistance={2}
          maxDistance={20}
          minPolarAngle={Math.PI / 2 - Math.PI / 18}
          maxPolarAngle={Math.PI / 2 + Math.PI / 18}
          minAzimuthAngle={-Math.PI / 18}
          maxAzimuthAngle={Math.PI / 18}
        />
        <Environment preset="city" />
      </Canvas>
    </div>
  );
}
